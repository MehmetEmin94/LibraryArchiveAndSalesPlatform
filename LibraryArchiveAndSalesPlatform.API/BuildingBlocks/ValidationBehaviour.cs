using FluentValidation;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Account;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Book;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Note;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Shelf;

namespace LibraryArchiveAndSalesPlatform.API.BuildingBlocks
{
    #region  Account validations
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        }
    }

    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").EmailAddress();
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required").MinimumLength(6);
        }
    }

    public class UserDetailsDtoValidator : AbstractValidator<UserDetailsDto>
    {
        public UserDetailsDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").EmailAddress();
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required");
        }
    }

    public class UserRoleValidator : AbstractValidator<UserRole>
    {
        public UserRoleValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required");
            RuleFor(x => x.Role).NotEmpty().WithMessage("Role is required");
        }
    }

    #endregion


    #region Book validations
    public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
    {
        public CreateBookDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Genre).NotEmpty().WithMessage("Genre is required");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
            RuleFor(x => x.ShelfId).NotEmpty().WithMessage("ShelfId is required");
            RuleFor(x => x.File).NotNull().WithMessage("File is required");
        }
    }

    public class UpdateBookDtoValidator : AbstractValidator<UpdateBookDto>
    {
        public UpdateBookDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Genre).NotEmpty().WithMessage("Genre is required");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
            RuleFor(x => x.ShelfId).NotEmpty().WithMessage("ShelfId is required");
            RuleFor(x => x.File).NotNull().WithMessage("File is required");
        }
    }

    #endregion


    #region Note validaions
    public class CreateNoteDtoValidator : AbstractValidator<CreateNoteDto>
    {
        public CreateNoteDtoValidator()
        {
            RuleFor(x => x.BookId).NotEmpty().WithMessage("BookId is required");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Content is required");
        }
    }

    public class UpdateNoteDtoValidator : AbstractValidator<UpdateNoteDto>
    {
        public UpdateNoteDtoValidator()
        {
            RuleFor(x => x.BookId).NotEmpty().WithMessage("BookId is required");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Content is required");
        }
    }
    #endregion


    #region Shelf validations
    public class CreateShelfDtoValidator : AbstractValidator<CreateShelfDto>
    {
        public CreateShelfDtoValidator()
        {
            RuleFor(x => x.Section).NotEmpty().WithMessage("Section is required");
            RuleFor(x => x.Row).NotEmpty().WithMessage("Row is required");
            RuleFor(x => x.Position).NotEmpty().WithMessage("Position is required");
        }
    }

    public class UpdateShelfDtoValidator : AbstractValidator<UpdateShelfDto>
    {
        public UpdateShelfDtoValidator()
        {
            RuleFor(x => x.Section).NotEmpty().WithMessage("Section is required");
            RuleFor(x => x.Row).NotEmpty().WithMessage("Row is required");
            RuleFor(x => x.Position).NotEmpty().WithMessage("Position is required");
        }
    }
    #endregion
}
