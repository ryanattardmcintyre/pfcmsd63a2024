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


        //blogs/<blog-id>/posts/<post-id>
        public async Task<IActionResult> Read(string postId, string blogId)
        {
            var myPost = await _postsRepository.GetPost(blogId, postId);
            return View(myPost);
        }

    }
}
