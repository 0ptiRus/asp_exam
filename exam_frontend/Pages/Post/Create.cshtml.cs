using System.Collections;
using System.Net.Http.Headers;
using System.Security.Claims;
using exam_frontend.Entities;
using exam_frontend.Models;
using exam_frontend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace exam_frontend.Pages.Post;

[Authorize]
public class Create : PageModel
{
    private readonly IApiService api;

    public Create(IApiService api)
    {
        this.api = api;
    }

    [BindProperty] public CreatePostModel Model { get; set; }
    public int SelectedGalleryId { get; set; }
    public IEnumerable<PreviewGalleryModel> Galleries { get; set; }
    

    public bool CanUpload { get; set; }

    public async Task<IActionResult> OnGetAsync(int gallery_id)
    {
        // Entities.Gallery gallery =
        //     await gallery_service.GetUserGallery(User.FindFirstValue(ClaimTypes.NameIdentifier)!, gallery_id);
        //
        // Entities.Gallery gallery = null;
        // HttpResponseMessage response = await api.GetAsync($"Galleries/{gallery_id}");
        // if (response.IsSuccessStatusCode)
        // {
        //     gallery = api.JsonToContent<Entities.Gallery>(await response.Content.ReadAsStringAsync());
        // }
        //
        // if (gallery == null)
        // {
        //     CanUpload = false;
        // }
        // else
        // {
        //     CanUpload = true;
        //     GalleryId = gallery_id;
        // }

        HttpResponseMessage response = await api.GetAsync($"Galleries/{User.FindFirstValue("nameid")}");
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return RedirectToPage("/Account/Login");
        }

        if (response.IsSuccessStatusCode)
        {
            Galleries = api.JsonToContent<IList<PreviewGalleryModel>>(await response.Content.ReadAsStringAsync());
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Model.File == null || Model.File.Length == 0)
        {
            ModelState.AddModelError("ImageFile", "Please select a file.");
            return Page();
        }
        
        String[] allowed_types = { "image/jpeg", "image/png" };
        if (!Array.Exists(allowed_types, type => type == Model.File.ContentType))
        {
            ModelState.AddModelError("ImageFile", "Only JPEG and PNG images are allowed.");
            return Page();
        }
        
        const int max_size = 10 * 1024 * 1024; // 20MB
        if (Model.File.Length > max_size)
        {
            ModelState.AddModelError("ImageFile", "File size must be under 20MB.");
            return Page();
        }
        
        using var content = new MultipartFormDataContent();

        content.Add(new StringContent(Model.Name), "Name");
        content.Add(new StringContent(Model.Description), "Description");
        content.Add(new StringContent(Model.GalleryId.ToString()), "GalleryId");
        content.Add(new StringContent(Model.Tags), "Tags");
        content.Add(new StringContent(User.FindFirstValue("nameid")), "UserId");
        
        var fileContent = new StreamContent(Model.File.OpenReadStream());
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(Model.File.ContentType);
        content.Add(fileContent, "File", Model.File.FileName);

        await api.PostAsync("Posts/", content);
        //await image_service.PostImage(ImageFile, GalleryId);

        return RedirectToPage("/Gallery/Details", 
            new { user_id = User.FindFirstValue("nameid"), gallery_id = Model.GalleryId });
    }

}