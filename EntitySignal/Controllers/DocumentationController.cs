﻿using EntitySignal.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EntitySignal.Controllers
{

  [Route("Documentation")]
  public class DocumentationController : Controller
  {
    private readonly IWebHostEnvironment _hostingEnvironment;

    public DocumentationController(
      IWebHostEnvironment hostingEnvironment
      )
    {
      _hostingEnvironment = hostingEnvironment;
    }

    public async Task<IActionResult> Index()
    {
      return await Get(null);
    }

    [HttpGet("{requestedDocmentation}")]
    public async Task<IActionResult> Get(string requestedDocmentation)
    {
      string docsDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, "Docs");
      var markdownFiles = Directory.GetFiles(docsDirectory)
        .Select(x=>Path.GetFileNameWithoutExtension(x))
        .OrderBy(x=>x);

      if(requestedDocmentation == null)
      {
        requestedDocmentation = markdownFiles.First();
      }
      else if(markdownFiles.Contains(requestedDocmentation) == false)
      {
        return BadRequest();
      }

      var requestedFile = $"{requestedDocmentation}.md";
      var requestedFilePath = Path.Combine(docsDirectory, requestedFile);
      var markdownFileText = await System.IO.File.ReadAllTextAsync(requestedFilePath);

      var documentationDisplayContainer = new DocumentationViewModel
      {
        Docs = markdownFiles,
        Markdown = markdownFileText,
        Title = requestedDocmentation.Replace("-", " "),
        RequestedDoc = requestedDocmentation
      };

      return View("Get", documentationDisplayContainer);
    }

  }
}
