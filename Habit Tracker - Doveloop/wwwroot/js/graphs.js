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
    start_cap = start_cap.toLowerCase()
    end_cap = end_cap.toLowerCase()

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

    let last_end = "north"
    for (let i = 0; i < data.length; i++) {
        //If colors are defined in a list, use the list
        let fillColor = typeof fill === "string" ? fill : fill[i]
        let strokeColor = typeof stroke === "string" ? stroke : stroke[i]

        let next_end = data[i] / data_sum * (2 * Math.PI) //Get a portion for the current data

        next_end += last_end === "north" ? 0 : last_end //Adjust the next end by the last end

        last_end = drawSubCircle(context, display_width / 2, display_height / 2, outer_radius, last_end, next_end, fillColor, strokeColor)
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

    /*Debug lines
    {
        drawLine(context, 0, 0, width, 0, "red")
        drawLine(context, 0, 0, 0, height, "red")
        drawLine(context, 0, height, width, height, "red")
        drawLine(context, width, 0, width, height, "red")

        drawLine(context, left_margin, margin, width - margin, margin, "orange")
        drawLine(context, left_margin, margin, left_margin, height - margin, "orange")
        drawLine(context, left_margin, height - margin, width - margin, height - margin, "orange")
        drawLine(context, width - margin, margin, width - margin, height - margin, "orange")
    }
    */

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
        context.fillText(data[0], startPoint.x, startPoint.y + labelOffset);
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
        context.fillText(data[1], lastPoint.x, lastPoint.y + labelOffset);
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
            context.fillText(data[i], lastPoint.x, lastPoint.y + labelOffset);
        }
    }
}

let graph_target = document.getElementById("graph_canvas")

drawPieChartFromDP(graph_target, getAllHabits())


