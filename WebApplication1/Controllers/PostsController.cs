using Microsoft.AspNetCore.Mvc;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    public class PostsController : Controller
    {
        PostsRepository _postsRepository;
        public PostsController(PostsRepository postsRepository) { 
        _postsRepository= postsRepository;  
        
        }

        public async Task<IActionResult> Index(string blogId)
        {
            var list = await _postsRepository.GetPosts(blogId);
            return View(list);
        }
    }
}
