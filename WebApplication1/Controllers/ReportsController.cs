using Microsoft.AspNetCore.Mvc;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    public class ReportsController : Controller
    {
        private PubsubRepository _pubsubRepository;
        private PostsRepository _postsRepository;
        public ReportsController(PubsubRepository pubsubRepository,
            PostsRepository postsRepository) {
        _pubsubRepository= pubsubRepository;
            _postsRepository = postsRepository;
        }

        public async Task<IActionResult> Generate()
        {
            string blogId =  _pubsubRepository.PullMessagesSync("msd63a2024ra-sub", true);

            if (string.IsNullOrEmpty(blogId)==false)
            {
                //we have blogid with data

                //first we get a list of posts
                //then we form the pdf
                var listOfPosts = await _postsRepository.GetPosts(blogId);

                //putting everything inside a pdf.




            }


            return View();
        }
    }
}
