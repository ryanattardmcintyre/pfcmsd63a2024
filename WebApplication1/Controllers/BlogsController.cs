using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    public class BlogsController : Controller
    {
        //now that we registered the BlogsRepository with the services collection, we can ask for it to be received here and
        //consumed: DEPENDENCY INJECTION (CONSTRUCTOR INJECTION)
        //WHY? this is a design pattern to promote code efficiency

        //Dependency Injection is used to centralize instances so we do not need to create a new instance everytime in every method

        private BlogsRepository _blogsRepository;
        public BlogsController(BlogsRepository blogsRepository) { 
        _blogsRepository= blogsRepository;
        }


        public async Task<IActionResult> Index()
        {
            var list = await _blogsRepository.GetBlogs();
            return View(list);
        
        }

        
        
        /// <summary>
        /// is called initially to LOAD the page where the user will be typing the inputs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public IActionResult Create() { return View(); }

        /// <summary>
        /// is called when the user has finished typing the data and clicked on submit
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public IActionResult Create(Blog b) {

            
            b.Id = Guid.NewGuid().ToString();
            b.DateCreated = Timestamp.FromDateTime(DateTime.UtcNow);
            b.DateUpdated = Timestamp.FromDateTime(DateTime.UtcNow);
            b.Author = User.Identity.Name;

            _blogsRepository.AddBlog(b);


            return View(); 
        }
    }
}
