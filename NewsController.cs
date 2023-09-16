using System;
using System.IO;
using DAL;
using Haberler.MyResult;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Image = DAL.Image;
using Haberler.Auth;
using DAL.Repository;
using Haberler.Extensions;

namespace Haberler.Controllers
{
    [AuthorizeEditor]
    public class NewsController : Controller
    {
        NewsRepository newsRepository;
        ImageRepository imageRepository;
        CategoryRepository categoryRepository;
        CommentRepository commentRepository;
        NewsContenRepository newsContenRepository;
        SliderRepository sliderRepository;

        public NewsController(NewsRepository newsRepository,
                              ImageRepository imageRepository,
                              CategoryRepository categoryRepository,
                              CommentRepository commentRepository,
                              NewsContenRepository newsContenRepository,
                              SliderRepository sliderRepository)
        {
            this.newsRepository = newsRepository;
            this.imageRepository = imageRepository;
            this.categoryRepository = categoryRepository;
            this.commentRepository = commentRepository;
            this.newsContenRepository = newsContenRepository;
            this.sliderRepository = sliderRepository;
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
