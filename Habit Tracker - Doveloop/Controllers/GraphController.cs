﻿using Microsoft.AspNetCore.Mvc;
using Habit_Tracker___Doveloop.Models;

namespace Habit_Tracker___Doveloop.Controllers
{
    public class GraphController : Controller
    {
        public IActionResult Index()
        {
            return View(new Graph(
                new GraphData[]
                {
                    new GraphData(5,    "Personal",     "#a2de6b"),
                    new GraphData(10,   "Professional", "#ffb763"),
                    new GraphData(15,   "Huh?",         "#be81f1"),
                    new GraphData(20,   "What?",        "#4de79e"),
                },
                new GraphData[]
                {
                    new GraphData(3,    "Personal",     "#aac090"),
                    new GraphData(7,    "Professional", "#c0aa90"),
                    new GraphData(12,   "Huh?",         "#aa90c0"),
                    new GraphData(15,   "What?",        "#90c0aa"),
                }
                )
            );
        }
    }
}
