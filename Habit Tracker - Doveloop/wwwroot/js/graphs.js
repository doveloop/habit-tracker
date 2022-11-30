/**
 * Scrubs string from C#
 */
let scrubQuotes = function (str)
{
    const regex = /&quot;/g
    return str.replace(regex, "\"")
}

/**
 * Camel-caser
 */
let toCamelCase = function (str)
{
    return str.split(" ").join("_")
}

/**
 * Returns a color from a string's hash
 * based on esmiralha's StackOverflow response (https://stackoverflow.com/a/7616484)
 */
let getColorFromString = function (str)
{
    let output = "#"
    let hash = 0
    let chr

    if (str.length === 0) return "#000"

    for(let i = 1; i <= 3; i++)
    {
        for (let j = (str.length / 3) * (i - 1); j < (str.length / 3) * i; j++)
        {
            chr = str.charCodeAt(j);
            hash = ((hash << 5) - hash) + chr;
            hash |= 0; // Convert to 32bit integer
        }
        let next = "00" + Math.abs(hash % 256).toString(16)
        next = next.slice(-2)

        output = `${output}${next}`
    }

    return output
}

/**
 * Choose text color based on a background color
 * based on Mark Ransom's StackOverflow response  (https://stackoverflow.com/a/3943023)
 */
let getTextColorFromBackground = function(background)
{
    let output
    let red = parseInt(background.slice(1,3), 16)
    let green = parseInt(background.slice(3,5), 16)
    let blue = parseInt(background.slice(5,7), 16)

    output = (red*0.299 + green*0.587 + blue*0.114) > 150 ? "#000000" : "#ffffff"

    return output
}

/**
 * Draws a rectangle to the specified HTML5 Canvas 2d context
 * @param context - HTML5 canvas 2d context
 * @param x
 * @param y
 * @param w
 * @param h
 * @param fill - HTML color
 * @param stroke - HTML color
 */
let drawRect = function (context, x, y, w, h, fill, stroke) {
    context.beginPath()
    context.rect(x, y, w, h)
    context.fillStyle = fill
    context.fill()
    context.strokeStyle = stroke
    context.stroke()
}

/**
 * Draws a subcircle (partial or entire circle, e.g. a 'pie' with slices out) to the specified HTML5 Canvas 2d context
 * @param context - HTML5 canvas 2d context
 * @param x
 * @param y
 * @param r - radius
 * @param sR - starting angle (in RADIANS), may also specify "north" to start circle at PI/2 radians
 * @param eR - ending angle (in RADIANS)
 * @param fill - HTML color
 * @param stroke - HTML color
 */
let drawSubCircle = function (context, x, y, r, sR, eR, fill, stroke) {
    if (sR === "north") {
        sR = -0.5 * Math.PI
        eR += sR
    }

    context.beginPath()

    let full_circle = eR >= 2 * Math.PI + sR

    if (!full_circle) // || (eR % (Math.PI * 2) !== sR % (Math.PI * 2))
    {
        context.moveTo(x, y)
    }

    context.arc(x, y, r, sR, eR)

    if (!full_circle) // || (eR % (Math.PI * 2) !== sR % (Math.PI * 2))
    {
        context.closePath()
    }

    context.fillStyle = fill
    context.fill()
    context.strokeStyle = stroke
    context.stroke()

    return eR
}

/**
 * Draws a line to the specified HTML5 Canvas 2d context
 * @param context - HTML5 canvas 2d context
 * @param x1
 * @param y1
 * @param x2
 * @param y2
 * @param stroke
 * @param start_cap - "circle," "vline" (vertical line), or "x"
 * @param end_cap - "circle," "vline" (vertical line), or "x"
 */
let drawLine = function (context, x1, y1, x2, y2, stroke, start_cap, end_cap) {
    context.beginPath()
    context.moveTo(x1, y1)

    const cap_size = 5

    start_cap = start_cap ? start_cap.toLowerCase() : undefined
    end_cap = end_cap ? end_cap.toLowerCase() : undefined

    if (start_cap) {
        if (start_cap === "circle") {
            drawSubCircle(context, x1, y1, cap_size / 2, 0, 2 * Math.PI, stroke, stroke)
        }
        else if (start_cap === "vline") {
            drawLine(context, x1, y1 - cap_size, x1, y1 + cap_size, stroke)
        }
        else if (start_cap === "x") {
            drawLine(context, x1 - (cap_size / 2), y1 - (cap_size / 2), x1 + (cap_size / 2), y1 + (cap_size / 2), stroke)
            drawLine(context, x1 - (cap_size / 2), y1 + (cap_size / 2), x1 + (cap_size / 2), y1 - (cap_size / 2), stroke)
        }
    }

    context.moveTo(x1, y1)
    context.lineTo(x2, y2)
    context.strokeStyle = stroke
    context.stroke()
    context.closePath()

    if (end_cap) {
        if (end_cap === "circle") {
            drawSubCircle(context, x2, y2, cap_size / 2, 0, 2 * Math.PI, stroke, stroke)
        }
        else if (end_cap === "vline") {
            drawLine(context, x2, y2 - cap_size, x2, y2 + cap_size, stroke)
        }
        else if (end_cap === "x") {
            drawLine(context, x2 - (cap_size / 2), y2 - (cap_size / 2), x2 + (cap_size / 2), y2 + (cap_size / 2), stroke)
            drawLine(context, x2 - (cap_size / 2), y2 + (cap_size / 2), x2 + (cap_size / 2), y2 - (cap_size / 2), stroke)
        }
    }

    return {
        x: x2,
        y: y2
    }
}

function DataPoint(value, label, fill_color, stroke_color) {
    if (!value || !label) {
        throw "Datapoint needs a value and a label"
    }

    this.value = parseInt(value)
    this.label = label
    this.fillColor = fill_color
    this.strokeColor = stroke_color
}

/**Fills the canvas element with a simple bar chart.
 * No labels are included, but the colors can be determined.
 *  canvas element to draw the chart in
 *  data a list containing the values to be charted
 *  labels a list containing the strings that describe the parallel data values
 *  fill a single color string for the fill of each bar OR a list corresponding to each data point
 *  stroke a single color string for the line stroke about the bars and the labels
 */
let drawBarChart = function (canvas, data, labels, fill, stroke) {
    const context = canvas.getContext("2d")

    const margin = 10
    const bar_padding = 5

    const width = canvas.getBoundingClientRect().width
    const height = canvas.getBoundingClientRect().height
    const display_width = (width - (2 * margin))
    const display_height = (height - (2 * margin))

    let bars = data.length

    const font_size = (display_width / bars) * 0.13

    context.font = font_size + "px Arial"
    context.textAlign = "center";

    let max = 0
    for (let i = 0; i < data.length; i++) {
        max = Math.max(max, data[i])
    }

    let height_factor = display_height / max

    let bar_width = (display_width - ((bars - 1) * bar_padding)) / bars

    let bottom_left = height - margin

    for (let i = 0; i < data.length; i++) {
        let curr_x = margin + (i * (bar_width + bar_padding))

        //If colors are defined in a list, use the list
        let fillColor = typeof fill === "string" ? fill : fill[i]
        let strokeColor = typeof stroke === "string" ? stroke : stroke[i]

        drawRect(canvas.getContext("2d"), curr_x, bottom_left, bar_width, -data[i] * height_factor, fillColor, strokeColor)

        //Do labels
        context.fillStyle = strokeColor
        // context.fillStyle = fillColor
        context.fillText(labels[i], curr_x + (bar_width / 2), display_height);
    }
}

let drawBarChartFromDP = function (canvas, data_points) {
    let data = []
    let fill = []
    let stroke = []
    let labels = []

    for (let i = 0; i < data_points.length; i++) {
        data.push(data_points[i].value)
        fill.push(data_points[i].fillColor)
        stroke.push(data_points[i].strokeColor)
        labels.push(data_points[i].label)
    }

    drawBarChart(canvas, data, labels, fill, stroke)
}

let drawPieChart = function (canvas, scale, data, labels, fill, stroke, alt_sum) {
    const context = canvas.getContext("2d")

    const margin = 20

    const width = canvas.getBoundingClientRect().width
    const height = canvas.getBoundingClientRect().height
    const display_width = (width - (2 * margin))
    const display_height = (height - (2 * margin))
    const outer_radius = Math.min(display_width / 2, display_height / 2) * scale

    let data_sum = 0
    for (let i = 0; i < data.length; i++) {
        data_sum += data[i]
    }

    if (alt_sum) { data_sum = alt_sum }

    const font_size = (display_width - outer_radius) * 0.025

    context.font = font_size + "px Arial"
    context.textAlign = "center";

    let last_end = "north"
    for (let i = 0; i < data.length; i++) {
        //If colors are defined in a list, use the list
        let fillColor = typeof fill === "string" ? fill : fill[i]
        let strokeColor = typeof stroke === "string" ? stroke : stroke[i]

        let next_end = data[i] / data_sum * (2 * Math.PI) //Get a portion for the current data

        next_end += last_end === "north" ? 0 : last_end //Adjust the next end by the last end

        last_end = drawSubCircle(context, display_width / 2, display_height / 2, outer_radius, last_end, next_end, fillColor, strokeColor)

        const keySize = 20
        drawRect(canvas.getContext("2d"), margin, (margin * i) + (keySize / 4), keySize - 2, keySize - 2, fillColor, "black")

        context.strokeStyle = "black"
        context.fillStyle = "black"
        context.textAlign = "left"
        context.fillText(labels[i],
            margin + keySize,
            (margin * i) + keySize)
    }
}

let drawPieChartFromDP = function (canvas, data_points, alt_sum, scale = 1.0) {
    let data = []
    let fill = []
    let stroke = []
    let labels = []

    for (let i = 0; i < data_points.length; i++) {
        data.push(data_points[i].value)
        fill.push(data_points[i].fillColor)
        stroke.push(data_points[i].strokeColor)
        labels.push(data_points[i].label)
    }

    drawPieChart(canvas, scale, data, labels, fill, stroke, alt_sum)
}

let drawNestedPieChartFromDP = function (canvas, actual_data_points, planned_data_points) {
    //Success counter
    let success = true

    //Draw the actual data but smaller and to the scale of the planned data
    let sum_planned_data = 0

    for (let i = 0; i < planned_data_points.length; i++) {
        sum_planned_data += planned_data_points[i].value

        //If any planned data point is larger than the actual, there is not success
        if (planned_data_points[i].value > actual_data_points[i].value) {
            success = false
        }
    }

    //Draw planned data
    drawPieChartFromDP(canvas, planned_data_points)

    //Draw separating circle
    drawPieChart(canvas, 0.9, [1], [""], success ? "#64e564" : "#fff", "black")

    //Draw grey circle
    drawPieChart(canvas, 0.8, [1], [""], "#eee", "black")

    drawPieChartFromDP(canvas, actual_data_points, sum_planned_data, 0.8)
}

let drawLineChart = function (canvas, scale, data, labels, fill, stroke, caps) {
    const context = canvas.getContext("2d")

    const margin = 10
    const left_margin = 50

    const width = canvas.getBoundingClientRect().width
    const height = canvas.getBoundingClientRect().height
    const display_width = (width - (2 * margin))
    const display_height = (height - (2 * margin))

    stroke = "black"

    let data_max = 0
    for (let i = 0; i < data.length; i++) {
        data_max = Math.max(data_max, data[i])
    }

    let vertical_factor = Math.max(5, Math.floor(Math.log2(data_max)))

    let subdivision_height = Math.ceil(data_max / vertical_factor)
    let notch_height = (subdivision_height / data_max) * display_height

    for (let i = 0; i < vertical_factor; i++) {
        let notch = (subdivision_height * i)
        let current_notch_y = (height - margin) - (i * notch_height)

        context.textAlign = "left"
        context.fillText(notch.toString(), margin, current_notch_y - 5)
        context.lineWidth = 1
        drawLine(context, margin, current_notch_y, left_margin, current_notch_y)
        context.setLineDash([1, 10])
        drawLine(context, left_margin, current_notch_y, display_width, current_notch_y)
        context.setLineDash([10, 0])
    }

    let bin_width = (display_width - (left_margin)) / (data.length - 1)
    // let labelOffset = 15
    let labelOffset = undefined

    let startPoint = {
        x: left_margin,
        y: display_height - ((data[0] / data_max) * (display_height - margin))
    }

    if (labelOffset) {
        context.textAlign = "center"
        context.fillText(data[0], startPoint.x, startPoint.y + labelOffset)
    }

    context.lineWidth = 2
    let lastPoint = drawLine(context,
        startPoint.x,
        startPoint.y,
        left_margin + bin_width,
        display_height - ((data[1] / data_max) * (display_height - margin)),
        stroke,
        caps, caps)

    if (labelOffset) {
        context.textAlign = "center"
        context.fillText(data[1], lastPoint.x, lastPoint.y + labelOffset)
    }
    for (let i = 2; i < data.length; i++) {
        context.lineWidth = 2
        lastPoint = drawLine(context,
            lastPoint.x,
            lastPoint.y,
            lastPoint.x + bin_width,
            display_height - ((data[i] / data_max) * (display_height - margin)),
            stroke,
            undefined, caps)
        if (labelOffset) {
            context.textAlign = "center"
            context.fillText(data[i], lastPoint.x, lastPoint.y + labelOffset)
        }
    }
}

let drawLineChartFromDP = function (canvas, data_points, caps) {
    let data = []
    let fill = []
    let stroke = []
    let labels = []

    for (let i = 0; i < data_points.length; i++) {
        data.push(data_points[i].value)
        fill.push(data_points[i].fillColor)
        stroke.push(data_points[i].strokeColor)
        labels.push(data_points[i].label)
    }

    drawLineChart(canvas, undefined, data, labels, fill, stroke, caps)
}

let graph_title = document.getElementById("graph_title")
let graph_target = document.getElementById("graph_canvas")

let serveGraph = function (data, filters, graphType)
{
    graphType = graphType.toLowerCase()
    let title = "Context"

    let habitData = data
    let habitDataPoints = []

    for(let i = 0; i < habitData.length; i++)
    {
        if(habitData[i].type === "habit")
        {
            let fillColor
            if(habitData[i].labels[0])
            {
                fillColor = getColorFromString(habitData[i].labels[0].name)
            }
            else
            {
                fillColor = getColorFromString(habitData[i].name)
            }

            let strokeColor = getTextColorFromBackground(fillColor)

            let newPoint = new DataPoint((Math.random() * 10) + 1 /*TODO GET ACTUAL ENTRIES*/, habitData[i].name, fillColor, strokeColor)

            //Using inclusive filter
            let includeInFilter = false
            let filtersSet = false

            //Check if any checked labels are present in this habit
            if(filters.labels.length > 0)
            {
                filtersSet = true

                for (let j = 0; j < habitData[i].labels.length; j++)
                {
                    if(filters.labels.indexOf(habitData[i].labels[j].name) >= 0)
                    {
                        includeInFilter = true
                    }
                }
            }

            if(includeInFilter || (!includeInFilter && !filtersSet))
            {
                habitDataPoints.push(newPoint)
            }
        }
    }

    graph_target.getContext("2d").clearRect(0, 0, graph_target.width, graph_target.height)

    switch (graphType)
    {
        case "bar":
            graph_title.innerText = `Bar Chart - ${title}`
            drawBarChartFromDP(graph_target, habitDataPoints)

            break;
        case "pie":
            graph_title.innerText = `Pie Chart - ${title}`
            drawPieChartFromDP(graph_target, habitDataPoints)

            break;
        case "line":
            graph_title.innerText = `Line Chart - ${title}`
            drawLineChartFromDP(graph_target, habitDataPoints, "x")

            break;
        default:
            console.log("Invalid graph type:", graphType)
            break;
    }
}

let copyGraph = function ()
{
    //Credit: Adriano Tumminelli on StackOverflow (https://stackoverflow.com/a/57546936)
    graph_target.toBlob(function(blob) {
        const item = new ClipboardItem({ "image/png": blob })
        navigator.clipboard.write([item])
    })
}

let saveGraph = function (link)
{
    link.setAttribute('download', graph_title.innerText+'.png');
    link.setAttribute('href', graph_target.toDataURL("image/png").replace("image/png", "image/octet-stream"));
}

//Only executes on the Graphs page
if(getAllHabits)
{
    let habitData = getAllHabits()
    let labels = []

    let filters = {
        labels: [],
        dateFrom: -1,
        dateTo: -1
    }

    for (let i = 0; i < habitData.length; i++)
    {
        for (let j = 0; j < habitData[i].labels.length; j++)
        {
            if(labels.indexOf(habitData[i].labels[j].name) === -1)
            {
                labels.push(habitData[i].labels[j].name)
            }
        }
    }

    serveGraph(habitData, filters, "pie") //TODO this is where the "last viewed" will go

    let graphButtons = document.querySelectorAll("#chart_filter>input[type=button]")
    for (let i = 0; i < graphButtons.length; i++)
    {
        let type = graphButtons[i].id.slice(15)

        graphButtons[i].addEventListener("click", function (event)
        {
            serveGraph(habitData, filters, type)
        })
    }

    document.getElementById("expand_share_menu").addEventListener("click", function (event) {
        document.getElementById("share_menu_dropdown").classList.toggle("hidden")
    })

    let copyGraphButton = document.getElementById("copy_graph")
    let saveGraphButton = document.getElementById("save_graph")

    copyGraphButton.addEventListener("click", function (event)
    {
        copyGraph()
        event.preventDefault()
    })
    saveGraphButton.addEventListener("click", function (event)
    {
        saveGraph(saveGraphButton)
    })

    let labelFilterWrapper = document.getElementById("label_filter_wrapper")
    let labelChecklist = document.getElementById("label_checklist")

    for (let i = 0; i < labels.length; i++)
    {
        let nextItem = document.createElement("input")
        nextItem.id = toCamelCase(labels[i])
        nextItem.name = toCamelCase(labels[i])
        nextItem.type = "checkbox"
        nextItem.value = labels[i]

        let nextLabel = document.createElement("label")
        nextLabel.for = toCamelCase(labels[i])
        nextLabel.innerText = labels[i]

        labelChecklist.appendChild(nextItem)
        labelChecklist.appendChild(nextLabel)
    }

    labelFilterWrapper.addEventListener("click", function (event) {
        labelChecklist.classList.toggle("hidden")
        event.preventDefault()
    })

    let dateStartFilterForm = document.getElementById("label_date_start")
    let dateEndFilterForm = document.getElementById("label_date_end")

    let chartUpdateFilterForm = document.getElementById("chart_filter")
    chartUpdateFilterForm.addEventListener("change", function (event)
    {
        let newLabelFilters = []
        for (let i = 0; i < labelChecklist.children.length; i++)
        {
            if(labelChecklist.children[i].checked)
            {
                newLabelFilters.push(labelChecklist.children[i].value)
            }
        }

        filters.labels = newLabelFilters

        filters.dateFrom = dateStartFilterForm.value === "" ? -1 : dateStartFilterForm.value
        filters.dateTo = dateEndFilterForm.value === "" ? -1 : dateEndFilterForm.value

    })
}
