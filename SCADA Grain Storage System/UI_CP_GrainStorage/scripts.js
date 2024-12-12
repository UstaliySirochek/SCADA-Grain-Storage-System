document.addEventListener("DOMContentLoaded", function () {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7026/indicator")
      .configureLogging(signalR.LogLevel.Information)
      .build();
  
    let unifiedChart;
    let unifiedChartData = {};
  
    async function start() {
      try {
        await connection.start();
        console.log("SignalR Connected.");
      } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
      }
    }
  
    connection.on("receive", (id, value) => {
      console.log(`Received value: ${value} for ID: ${id}`);
  
      axios.get(`https://localhost:7026/api/Indicator/${id}`).then((response) => {
        const { x, y } = response.data;
  
        displayIndicatorValue(id, value, x, y);
  
        updateUnifiedChart(id, parseFloat(value));
      });
    });
  
    connection.on("receiveTime", (time) => {
      const parsedTime = new Date(time);
      const formattedTime = parsedTime.toLocaleString();
      console.log(`Received time: ${formattedTime}`);
      const timeElement = document.getElementById("current-time");
      if (timeElement) {
        timeElement.textContent = formattedTime;
      }
    });
  
    connection.onclose(async () => {
      await start();
    });
  
    start();
  
    document.getElementById("create-indicator-form").onsubmit = onFormSubmit;
    document.getElementById("update-indicator-form").onsubmit = onFormUpdate;
  
    getIndicators();
  
    setInterval(() => {
      getIndicators();
    }, 1000);
  
    document.getElementById("backgroundArea").onclick = (e) => {
      const xInput = document.getElementById("x");
      const yInput = document.getElementById("y");
      if (xInput && yInput) {
        xInput.value = e.offsetX;
        yInput.value = e.offsetY;
      }
    };
  
    function getIndicators() {
      axios.get("https://localhost:7026/api/Indicator").then((result) => {
        result.data.forEach((indicator) => {
          const { id, value, x, y } = indicator;
  
          displayIndicatorValue(id, value, x, y);
  
          if (!unifiedChartData[indicator.id]) {
            createUnifiedChartDataset(indicator.id, indicator.name);
          }
          updateUnifiedChart(indicator.id, parseFloat(indicator.value));
        });
      });
    }
  
    function onFormSubmit(e) {
      e.preventDefault();
      if (e.submitter.value === "Create") {
        tryNewIndicator(
          e.target[0].value,
          e.target[1].value,
          e.target[2].value,
          e.target[3].value,
          e.target[4].value
        );
      }
    }
  
    function tryNewIndicator(name, value, unit, x, y) {
      axios
        .post("https://localhost:7026/api/Indicator", {
          name: name,
          description: name,
          x,
          y,
          value: value,
          unit,
        })
        .then((result) => {
          const indicator = result.data;
  
          if (!unifiedChartData[indicator.id]) {
            createUnifiedChartDataset(indicator.id, name);
          }
  
          displayIndicatorValue(indicator.id, value, x, y);
        });
    }
  
    function onFormUpdate(e) {
      e.preventDefault();
      if (e.submitter.value === "Update") {
        const param1 = e.target[0].value;
        const param2 = e.target[1].value;
        updateTargetValue(param1, param2);
      }
    }
  
    function changeBackgroundImage(id) {
      const element = document.getElementsByClassName("background-image")[0];
      element.src = "https://localhost:7026/api/BackgroundImage/" + id;
    }
  
    document
      .getElementById("changeBg")
      .addEventListener("submit", function (event) {
        event.preventDefault();
        const value = document.getElementById("changeBgNumber").value;
        changeBackgroundImage(value);
      });
  
    document
      .getElementById("imageForm")
      .addEventListener("submit", function (event) {
        event.preventDefault();
  
        const formData = new FormData();
        const fileInput = document.getElementById("imageInput").files[0];
  
        if (fileInput) {
          formData.append("image", fileInput);
          axios
            .post(
              "https://localhost:7026/api/backgroundimage/upload-image",
              formData
            )
            .then(() => {
              console.log("Image uploaded successfully");
            })
            .catch((error) => {
              console.error("Error : ", error);
            });
        } else {
          console.error("No file selected");
        }
      });
  
    function createUnifiedChart() {
      const chartContainer = document.createElement("div");
      chartContainer.style.marginTop = "20px";
      chartContainer.style.textAlign = "center";
  
      const chartCanvas = document.createElement("canvas");
      chartCanvas.id = "unified-chart";
      chartCanvas.style.width = "800px";
      chartCanvas.style.height = "400px";
      chartContainer.appendChild(chartCanvas);
  
      const form = document.getElementById("create-indicator-form");
      form.parentNode.insertBefore(chartContainer, form.nextSibling);
  
      unifiedChart = new Chart(chartCanvas.getContext("2d"), {
        type: "line",
        data: {
          labels: [],
          datasets: [],
        },
        options: {
          responsive: true,
          animation: false,
          maintainAspectRatio: false,
          scales: {
            x: { title: { display: true, text: "Time" } },
            y: {
              title: { display: true, text: "Value" },
              min: -40,
              max: 100,
            },
          },
        },
      });
    }
  
    function createUnifiedChartDataset(indicatorId, indicatorName) {
      if (!unifiedChart) {
        createUnifiedChart();
      }
  
      unifiedChartData[indicatorId] = {
        labels: [],
        data: [],
      };
  
      unifiedChart.data.datasets.push({
        label: `Indicator: ${indicatorName}`,
        data: unifiedChartData[indicatorId].data,
        borderColor: getRandomColor(),
        backgroundColor: "rgba(0, 0, 0, 0)",
        borderWidth: 2,
      });
      unifiedChart.update();
    }
  
    function updateUnifiedChart(indicatorId, value) {
      const currentTime = new Date().toLocaleTimeString();
      const history = unifiedChartData[indicatorId];
  
      history.labels.push(currentTime);
      history.data.push(value);
  
      if (history.labels.length > 20) {
        history.labels.shift();
        history.data.shift();
      }
  
      unifiedChart.data.labels = history.labels;
      unifiedChart.update();
    }
  
    function getRandomColor() {
      return `rgba(${Math.floor(
        Math.random() * 255
      )}, ${Math.floor(Math.random() * 255)}, ${Math.floor(Math.random() * 255)}, 1)`;
    }
  
    function displayIndicatorValue(id, value, x, y) {
      let indicatorElement = document.getElementById(`indicator-${id}`);
      if (!indicatorElement) {
        indicatorElement = document.createElement("div");
        indicatorElement.id = `indicator-${id}`;
        indicatorElement.className = "indicator-value";
        indicatorElement.style.position = "absolute";
        indicatorElement.style.left = `${x + 140}px`;
        indicatorElement.style.top = `${y - 15}px`;
        indicatorElement.style.backgroundColor = "rgba(255, 255, 255, 0.9)";
        indicatorElement.style.padding = "5px";
        indicatorElement.style.border = "1px solid #ccc";
        indicatorElement.style.borderRadius = "4px";
        indicatorElement.style.fontSize = "14px";
        indicatorElement.style.zIndex = "1000";
        document.body.appendChild(indicatorElement);
      }
  
      indicatorElement.textContent = `${value}`;
    }
  });