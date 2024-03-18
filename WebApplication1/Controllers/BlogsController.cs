using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
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
        private PubsubRepository _pubSubRepository;
        private BucketsRepository _bucketsRepository;
        private IConfiguration _config;
        public BlogsController(BlogsRepository blogsRepository, BucketsRepository bucketsRepository, IConfiguration config,
            PubsubRepository pubSubRepository
            ) { 
            _blogsRepository= blogsRepository;
            _bucketsRepository= bucketsRepository;
            _config= config;
            _pubSubRepository= pubSubRepository;
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
        public async Task<IActionResult> Create(Blog b, IFormFile file, string recipient) {

            string bucketName = _config["bucket"];

            string uniqueFilename = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);

            MemoryStream userFile = new MemoryStream();
            file.CopyTo(userFile);

            await _bucketsRepository.UploadFile(uniqueFilename, userFile); //uploads the file and we wait for it to complete
            await _bucketsRepository.GrantAccess(uniqueFilename, recipient); //grants the right to the specified user
            b.Photo = $"https://storage.cloud.google.com/{bucketName}/{uniqueFilename}";

            b.Id = Guid.NewGuid().ToString();
            b.DateCreated = Timestamp.FromDateTime(DateTime.UtcNow);
            b.DateUpdated = Timestamp.FromDateTime(DateTime.UtcNow);
            b.Author = User.Identity.Name;

            _blogsRepository.AddBlog(b);


            return View(); 
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit (string blogId)
        { 
            //unlike the CREATE we are NOT creating a new document here; we are editing an existent one which EXISTS

            //therefore i'm reading the blog to be edited and displaying the existent details onto the page
            var list = await (_blogsRepository.GetBlogs());
            var myBlogToEdit = list.SingleOrDefault(x => x.Id == blogId); //filtering and getting only the needed blog by id

            //optional: validation
            if (myBlogToEdit.Author != User.Identity.Name)
            {//that's not my blog to edit
                //access denied
                return RedirectToAction("Index");
            }

            return View(myBlogToEdit);  //returning it to the page to be edited
        
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(Blog updatedBlog, IFormFile file)
        {
            //i am assuming that the typed in a different name for the blog
            
            //validation can be repeated!

            //same code to upload image as implemented in the CREATE

            await _blogsRepository.UpdateBlog(updatedBlog);
            return View(updatedBlog); //OR if you want to redirect the user to another View: return RedirectToAction("Index");
        }


        [Authorize]
        public async Task<IActionResult> Delete(string blogId)
        {
            await _blogsRepository.DeleteBlog(blogId);
            return RedirectToAction("Index");
        }




        public async Task<IActionResult> CreateReport(string blogId)
        {
            await _pubSubRepository.Publish(blogId);
            TempData["message"] = "Process will be initiated soon. This will take time so you may continue browsing meanwhile...";
            //Tempdata is like ViewBag (a way how to pass stringified data to the user on the page) but this is not lost after
            //a redirection

            return RedirectToAction("Index");
        }

    }
}
