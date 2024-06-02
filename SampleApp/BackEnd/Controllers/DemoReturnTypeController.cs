using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace SampleApp.BackEnd.Controllers;

public class DemoReturnTypeController : ControllerBase
{
    [HttpGet("getobject")]
    public object GetObject()
    {
        return GetBookDto();
    }

    //return a 204 no content with no body
    //response type header is no set
    [HttpGet("getobjectnull")]
    public object GetObjectNull()
    {
        return null;
    }

    //will set response type to text/plain
    [HttpGet("getstring")]
    public string GetString()
    {
        return JsonSerializer.Serialize(GetBookDto());
    }

    [HttpGet("getjson")]
    public JsonResult GetJson()
    {
        return new JsonResult(GetBookDto());
    }

    [HttpGet("getiactionresult")]
    public IActionResult GetIActionResult()
    {
        return Ok(GetBookDto());
    }
    //Best practice to use ActionResult<T> instead of others ones
    [HttpGet("getiactionresult<T>")]
    public ActionResult<BookDto> GetIActionResultOfT()
    {
        return Ok(GetBookDto());
    }

    private BookDto GetBookDto()
    {
        return new BookDto
        {
            Id = 1,
            Title = "Book Title",
            Author = "Book Author"
        };
    }
}

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
}