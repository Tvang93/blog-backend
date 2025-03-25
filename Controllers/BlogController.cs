using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog.Models;
using blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace blog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly BlogServices _blogservices;
        public BlogController(BlogServices blogServices){
            _blogservices = blogServices;
        }

        [HttpGet("GetAllBlogs")]
        public async Task<IActionResult> GetAllBlogs(){
            var blogs = await _blogservices.GetBlogsAsync();
            if(blogs != null)return Ok(blogs);
            return BadRequest(new {Message = "No Blogs"});
        }

        [HttpPost("AddBlog")]
        public async Task<IActionResult> AddBlog([FromBody]BlogModel blog){
            var success = await _blogservices.AddBlogAsync(blog);
            if(success) return Ok(new{Success = true});
            return BadRequest(new{Message = "Blog was not added."});
        }

        [HttpPut("EditBlog")]
        public async Task<IActionResult> EditBlog([FromBody]BlogModel blog){
            var success = await _blogservices.EditBlogAsync(blog);
            if(success) return Ok(new{Success = true});
            return BadRequest(new{Message = "Blog was not edited."});
        }

        [HttpDelete("DeleteBlog")]
        public async Task<IActionResult> DeleteBlog([FromBody]BlogModel blog){
            var success = await _blogservices.EditBlogAsync(blog);
            if(success) return Ok(new{Success = true});
            return BadRequest(new{Message = "Blog was not edited."});
        }


        [HttpGet("GetBlogsByUserId/{userId}")]
        public async Task<IActionResult> GetBlogsByUserId(int userId){
            var blogs = await _blogservices.GetBlogsByUserIdAsync(userId);
            if(blogs != null)return Ok(blogs);
            return BadRequest(new {Message = "No Blogs."});
        }

        [HttpGet("GetBlogsByCategory/{category}")]
        public async Task<IActionResult> GetBlogsByCategory(string category){
            var blogs = await _blogservices.GetBlogsByCategory(category);
            if(blogs != null)return Ok(blogs);
            return BadRequest(new {Message = "No Blogs with That Category."});
        }

    }
}