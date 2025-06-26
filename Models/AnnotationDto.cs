using System;
using System.ComponentModel.DataAnnotations;
namespace FrontendApp.Models
{    public class AnnotationCreateDTO
    {
        [Required(ErrorMessage = "Le MenuId est requis.")]
        public int MenuId { get; set; }

        [Required(ErrorMessage = "La note est requise.")]
        [Range(1, 5, ErrorMessage = "La note doit être entre 1 et 5.")]
        public int Note { get; set; }

        [MaxLength(500, ErrorMessage = "Le commentaire ne peut pas dépasser 500 caractères.")]
        public string? Commentaire { get; set; } 
    }

    public class AnnotationUpdateDTO
    {
        [Required(ErrorMessage = "La note est requise.")]
        [Range(1, 5, ErrorMessage = "La note doit être entre 1 et 5.")]
        public int Note { get; set; }

        [MaxLength(500, ErrorMessage = "Le commentaire ne peut pas dépasser 500 caractères.")]
        public string? Commentaire { get; set; } 
    }

    public class AnnotationDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public int MenuId { get; set; }
        public int Note { get; set; }
        public string? Commentaire { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}