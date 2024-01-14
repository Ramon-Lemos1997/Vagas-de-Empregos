using Domain.Interfaces.Vagas;
using Domain.Models.Vagas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Presentation.Areas.Vagas.Controllers
{
    [Area("Vagas")]
    public class VagasController : Controller
    {
        private readonly IVagas _vagas;
        public VagasController(IVagas vagas)
        {
            _vagas = vagas;
        }

        //------------------------------------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            var (result, list) = await _vagas.ListAll(page);
            if (result.Success)
            {
                return View(list);
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(list);
        }

        [HttpGet]
        [Authorize]
        public IActionResult CreateVaga() => View();

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {

            var (result, vaga) = await _vagas.GetById(id);
            if (result.Success)
            {
                return View(vaga);
            }

            TempData["MessageError"] = result.Message;
            return View(vaga);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UserVagas()
        {
            var (result, list) = await _vagas.RetrieveVagasByUserID(User);
            if (result.Success)
            {
                return View(list);
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(list);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditVaga(int id)
        {
            var (result, vaga) = await _vagas.GetVagaForEditAndDeleteById(id);
            if (result.Success)
            {
                return View(vaga);
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeleteVaga(int id)
        {
            var (result, vaga) = await _vagas.GetVagaForEditAndDeleteById(id);
            if (result.Success)
            {
                return View(vaga);
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View();
        }

        //------------------------------------------------------------------------------------


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateVaga(VagaModel request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var result = await _vagas.Create(User, request);
            if (result.Success)
            {
                TempData["MessageSuccess"] = "Vaga cadastrada com sucesso.";
                return RedirectToAction(nameof(CreateVaga), "Vagas", new { area = "Vagas" });
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> UploadCurriculum(IFormFile curriculum, string userEmail, string title, int vagaId)
        {
            var result = await _vagas.SendCurriculum(curriculum, userEmail, title);
            if (result.Success)
            {
                TempData["MessageSuccess"] = "Currículo enviado com sucesso.";
                return RedirectToAction("Details", new { id = vagaId });
            }

            TempData["MessageError"] = result.Message;
            return RedirectToAction("Details", new { id = vagaId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditVaga(VagaModel request)
        {
            if (ModelState.IsValid)
            {
                var result = await _vagas.Edit(request);
                if (result.Success)
                {
                    TempData["MessageSuccess"] = "Vaga editada com sucesso.";
                    return RedirectToAction(nameof(EditVaga), new { id = request.Id });
                }

                ModelState.AddModelError(string.Empty, result.Message);
                return View();
            }

            return View(request);
        }

        [HttpPost, ActionName("DeleteVaga")]
        [Authorize]
        public async Task<IActionResult> DeleteVagaConfirmed(int vagaId)
        {
            var result = await _vagas.Delete(vagaId);
            if (result.Success)
            {
                TempData["MessageSuccess"] = "Vaga excluída com sucesso.";
                return RedirectToAction(nameof(UserVagas));
            }

            TempData["MessageError"] = result.Message;
            return RedirectToAction(nameof(UserVagas));
        }


        //------------------------------------------------------------------------------------
    }
}
