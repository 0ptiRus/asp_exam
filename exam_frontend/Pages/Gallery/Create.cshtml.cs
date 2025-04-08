using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using exam_frontend.Entities;
using exam_frontend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace exam_frontend.Pages.Gallery;

[Authorize]
public class Create : PageModel
{
    private readonly IApiService api;

    public Create(IApiService api)
    {
        this.api = api;
    }

    [BindProperty]
    public CreateGalleryModel Model { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Page();
        }
        //await service.CreateGallery(new(Model.Name, User.FindFirstValue(ClaimTypes.NameIdentifier)!, 
        //Model.IsPrivate));
        await api.PostAsJsonAsync($"Galleries/", Model);

        return RedirectToPage("/Gallery/Index", new { user_id = User.FindFirstValue(ClaimTypes.NameIdentifier)});
    }

    public class CreateGalleryModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public bool IsPrivate { get; set; }
        public string UserId { get; set; }
    }

}