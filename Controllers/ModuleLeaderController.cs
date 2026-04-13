using Microsoft.AspNetCore.Mvc;

public IActionResult Dashboard()
{
    ViewBag.TotalProjects = 10;
    ViewBag.TotalMatches = 5;
    ViewBag.PendingProjects = 3;
    ViewBag.TotalUsers = 20;

    return View();
}