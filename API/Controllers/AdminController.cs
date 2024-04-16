using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {   
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public AdminController(UserManager<AppUser> userManager, IUnitOfWork uow, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _uow = uow;
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles(){
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .Select(u => new {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles!.Select(r => r.Role!.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }
        
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery]string roles){
            if(string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");

            var selectedRoles = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);
            if(user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            var results = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if(!results.Succeeded) return BadRequest("Failed to add to roles");

            results = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if(!results.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-approve")]
        public async Task<ActionResult> GetPhotosForApproval(){
            var photosNotApproved = await _uow.PhotoRepository.GetUnapprovedPhotos();
            if(photosNotApproved == null) return NotFound();
            return Ok(photosNotApproved);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPut("approve-photo/{photoId}")]
        public async Task<ActionResult<PhotoDto>> ApprovePhoto(int photoId){
            var photo = await _uow.PhotoRepository.GetPhotoById(photoId);
            var user = await _uow.UserRepository.GetUserByPhotoIdAsync(photoId);

            if(photo == null) return NotFound();
            if(user == null) return NotFound();
            if(photo.IsApproved) return BadRequest("Photo already approved");
            
            var currentMain = user.Photos!.FirstOrDefault(x => x.IsMain);
            if(currentMain == null){
                photo.IsMain = true;
            }
            
            photo.IsApproved = true;
            
            if(await _uow.Complete()) return Ok(_mapper.Map<PhotoDto>(photo));

            return BadRequest("Failed to approve photo");
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpDelete("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId){
            var photo = await _uow.PhotoRepository.GetPhotoById(photoId);
            if(photo == null) return NotFound();

            if(photo.PublicId != null){
                var result = await _photoService.DeletePhotoAsync(photo.PublicId!);
                if(result.Error != null) return BadRequest(result.Error.Message);
            }

            _uow.PhotoRepository.RemovePhoto(photo);
            if(await _uow.Complete()) return Ok();

            return BadRequest("Problem rejecting photo");
        }

        [HttpGet("get-user-by-photo/{photoId}")]
        public async Task<ActionResult<MemberDto>> GetUserByPhoto(int photoId){
            var user = await _uow.UserRepository.GetUserByPhotoIdAsync(photoId);
            if(user == null) return NotFound();

            return Ok(user);
        }   

    }
}