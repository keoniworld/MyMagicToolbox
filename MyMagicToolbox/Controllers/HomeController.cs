using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using MagicFileFormats.Dec;
using MyMagicToolbox.Models;

namespace MyMagicToolbox.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult DeckList()
        {
            ViewBag.Message = "Your Deck List.";

            return View();
        }

        [HttpPost]
        public ActionResult UploadDeck(HttpPostedFileBase photo)
        {
            var model = new DeckListModel();
            try
            {
                string directory = Path.GetTempPath();

                if (photo != null && photo.ContentLength > 0)
                {
                    var fileName = Path.Combine(directory, Path.GetFileName(photo.FileName));
                    try
                    {
                        photo.SaveAs(fileName);

                        var reader = new DecReader();
                        var result = reader.ReadFile(fileName);

                        // TODO: Wrap result and add to model

                        model.Cards = result;
                    }
                    finally
                    {
                        var info = new FileInfo(fileName);
                        if (info.Exists)
                        {
                            info.Delete();
                        }
                    }
                }
            }
            catch (Exception error)
            {
            }

            // return RedirectToAction("DeckList");
            return View(model);
        }
    }
}