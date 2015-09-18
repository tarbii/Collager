using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Collager.Models;

namespace Collager.Controllers
{
    public class CollageController : Controller
    {
        // GET: Collage
        public async Task<ActionResult> Index(string login, 
            int height = 800, int width = 1200)
        {
            if (string.IsNullOrEmpty(login))
            {
                return HttpNotFound("Check your login");
            }

            var auth = await Twitter.Authorize();
            var users = await Twitter.ShowFriends(auth, login);
            var bitmap = Collage.CreateCollage(users, height, width);

            var converter = new ImageConverter();
            var byteArray = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));

            return View(byteArray);
        }
    }
}