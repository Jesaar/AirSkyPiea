using Microsoft.EntityFrameworkCore;
using AirSkyPiea1._0.Data.Entities;
using AirSkyPiea1._0.Helpers;
using Air_SkyPiea1._0.Enums;

namespace AirSkyPiea1._0.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;

        public SeedDb(DataContext context, IUserHelper userHelper, IBlobHelper blobHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCategoriesAsync();
            await CheckCountriesAsync();
            await CheckRolesAsync();
            await CheckUserAsync("1010", "Manuel", "Mesa", "manumesa@yopmail.com", "302 332 4356", "Avenida Caracas ", "noimage.png", UserType.Admin);
            await CheckUserAsync("2020", "Luciana", "Gomez", "lucy@yopmail.com", "309 339 3320", "Calle Luna Calle Sol", "noimage.png", UserType.User);
            await CheckDestinationsAsync();
        }

        private async Task CheckDestinationsAsync()
        {
            if (!_context.Destinations.Any())
            {
                await AddDestinationAsync("Bogotá DC", 270000M, 12F, new List<string>() { "Nacional" }, new List<string>() { "MuseoOroBogota.png" ,"Casas-de-Bogota-Colombia.png","Colibri-en-Bogota.png"});
                await AddDestinationAsync("Guatavita", 250000M, 12F, new List<string>() { "Nacional"}, new List<string>() { "Guatavita.png","EmbalseTomineGuatavita.png","lagunaGuatavita.png" });
                await AddDestinationAsync("Villa De Leyva", 1300000M, 12F, new List<string>() { "Nacional" }, new List<string>() { "VillaLeyva.png", "PozosAzulesVillaLeyva.png","ArquitecturaVillaLeyva.png" });
                await AddDestinationAsync("Monguí", 870000M, 12F, new List<string>() { "Nacional" }, new List<string>() { "Mongui.png","Monguib.png","Monguic.png"});
                await AddDestinationAsync("Desierto De La Tatacoa", 12000000M, 6F, new List<string>() { "Nacional" }, new List<string>() { "Tatacoa.png", "Tatacoab.png", "Tatacoac.png" });
                await AddDestinationAsync("Barichara", 56000M, 24F, new List<string>() { "Nacional" }, new List<string>() { "Barichara.png", "Baricharab.png" , "Baricharac.png" });
                await AddDestinationAsync("Parque Nacional Natural Chingaza", 820000M, 12F, new List<string>() { "Nacional" }, new List<string>() { "Chingaza.png", "Chingazab.png","Chingazac.png"});
                await AddDestinationAsync("San Gil", 5200000M, 6F, new List<string>() { "Nacional" }, new List<string>() { "SanGil.png", "SanGilb.png", "SanGilc.png" });
                await AddDestinationAsync("Medellín", 12100000M, 6F, new List<string>() { "Nacional" }, new List<string>() { "Medellin.png", "Medellinb.png" , "Medellinc.png" });
                await AddDestinationAsync("Gran Muralla, China", 15700000M, 12F, new List<string>() { "Internacional" }, new List<string>() { "MurallaChina.png", "MurallaChinab.png" , "MurallaChinac.png" });
                await AddDestinationAsync("Gran Pirámide de Guiza, Egipto", 26000M, 100F, new List<string>() { "Internacional" }, new List<string>() { "PiramideEgipto.png", "PiramideEgiptob.png" , "PiramideEgiptoc.png" });
                await AddDestinationAsync("Torre Eiffel,París, Francia", 180000M, 12F, new List<string>() { "Internacional"}, new List<string>() { "TorreEiffel.png", "TorreEiffelb.png", "TorreEiffelc.png" });
                await AddDestinationAsync("Estatua de la libertad, Nueva York, EE.UU", 179000M, 12F, new List<string>() { "Internacional" }, new List<string>() { "Libertad.png", "Libertadb.png", "Libertadc.png" });
                await AddDestinationAsync("Cancún y la Riviera Maya, México", 233000M, 12F, new List<string>() { "Internacional" }, new List<string>() { "Mexico.png", "Mexicob.png", "Mexicoc.png" });
                await AddDestinationAsync("DisneylandPark, Anaheim, EE.UU", 249900M, 12F, new List<string>() { "Internacional" }, new List<string>() { "Disney.png", "Disneyb.png", "Disneyc.png" });
               
                await _context.SaveChangesAsync();
            }
        }

        private async Task AddDestinationAsync(string name, decimal price, float stock, List<string> categories, List<string> images)
        {
            Destination destination = new()
            {
                Description = name,
                Name = name,
                Price = price,
                Stock = stock,
                DestinationCategories = new List<DestinationCategory>(),
                DestinationImages = new List<DestinationImage>()
            };

            foreach (string? category in categories)
            {
                destination.DestinationCategories.Add(new DestinationCategory { Category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == category) });
            }


            foreach (string? image in images)
            {
                Guid imageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\Destinations\\{image}", "destinations");
                destination.DestinationImages.Add(new DestinationImage { ImageId = imageId });
            }

            _context.Destinations.Add(destination);
        }

        private async Task<User> CheckUserAsync(
            string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            string image,
            UserType userType)
        {
            User user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                Guid imageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\Users\\{image}", "users");
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    City = _context.Cities.FirstOrDefault(),
                    UserType = userType,
                    ImageId = imageId
                };

                await _userHelper.AddUserAsync(user, "CursoDeZulu2020.");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new Country
                {
                    Name = "Colombia",
                    States = new List<State>()
            {
                new State()
                {
                    Name = "Cundinamarca",
                    Cities = new List<City>() {
                        new City() { Name = "Usaquén" },
                        new City() { Name = "Guatavita" },
                             new City() { Name = "Parque Nacional Natural Chingaza" },

                    }
                },
                new State()
                {
                    Name = "Boyacá",
                    Cities = new List<City>() {
                        new City() { Name = "Villa De Leyva" },
                        new City() { Name = "Monguí" },
                         new City() { Name = "Parque Nacional Natura El Cocuy" },

                    }
                },
                new State()
                {
                    Name = "Huila",
                    Cities = new List<City>() {
                        new City() { Name = "Desierto De La Tatacoa" },

                    }
                },
                new State()
                {
                    Name = "Santander",
                    Cities = new List<City>() {

                        new City() { Name = "Barichara" },

                        new City() { Name = "San Gil" },

                    }
                },
                 new State()
                {
                    Name = "Antioquia",
                    Cities = new List<City>() {

                        new City() { Name = "Medellín" },

                    }
            }
                }
                });
                _context.Countries.Add(new Country
                {
                    Name = "Estados Unidos",
                    States = new List<State>()
            {
                new State()
                {
                    Name = "Florida",
                    Cities = new List<City>() {
                        new City() { Name = "Orlando" },

                    }

            }
                }
                });
                _context.Countries.Add(new Country
                {
                    Name = "China",
                    States = new List<State>()
            {
                new State()
                {
                    Name = "Shanhaiguan",
                    Cities = new List<City>() {
                        new City() { Name = "Golfo de Bohay" },
                    }
                },
               
            }
                });
                _context.Countries.Add(new Country
                {
                    Name = "Egipto",
                    States = new List<State>()
            {
                new State()
                {
                    Name = "El Cairo",
                    Cities = new List<City>() {
                        new City() { Name = "Necrópolis de Guiza" },
                    }
                },

            }
                });
                _context.Countries.Add(new Country
                {
                    Name = "Francia",
                    States = new List<State>()
            {
                new State()
                {
                    Name = "París",
                    Cities = new List<City>() {
                        new City() { Name = "París" },
                    }
                },

            }
                });
                _context.Countries.Add(new Country
                {
                    Name = "México",
                    States = new List<State>()
            {
                new State()
                {
                    Name = "Cancún y la Riviera Maya",
                    Cities = new List<City>() {
                        new City() { Name = "Playa del Carmen" },
                    }
                },

            }
                });
              
            }

            await _context.SaveChangesAsync();
        }

        private async Task CheckCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Name = "Nacional" });
                _context.Categories.Add(new Category { Name = "Internacional" });
               
            }
        }
    }
}
