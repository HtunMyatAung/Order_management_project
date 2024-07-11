// ShopService.cs
using IdentityDemo.Models;
using IdentityDemo.Repositories;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Services
{
    public class ShopService : IShopService
    {
        private readonly IShopRepository _shopRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IUserRepository _userRepository;

        public ShopService(IShopRepository shopRepository, IWebHostEnvironment environment,IUserRepository userRepository)
        {
            _shopRepository = shopRepository;
            _environment = environment;
            _userRepository = userRepository;
        }
        public  IEnumerable<ShopModel> GetAllShopsAsync()
        {
            return  _shopRepository.GetAllShopsAsync();
        }

        public async Task<ShopModel> GetShopByIdAsync(int shopId)
        {
            return await _shopRepository.GetShopByIdAsync(shopId);
        }
        public async Task<List<ShopViewModel>> GetShopsNOwnersAsync()
        {
            var shops =  _shopRepository.GetAllShopsAsync();
            var users = _userRepository.GetUsers();
            return (from shop in shops
                    join user in users
                    on shop.ShopId equals user.ShopId // Assuming Shop has a ShopOwnerId property
                    select new ShopViewModel
                    {
                        ShopId = shop.ShopId,
                        ShopName = shop.ShopName,
                        ShopPhone = shop.ShopPhone,
                        ShopEmail = shop.ShopEmail,
                        ShopAddress = shop.ShopAddress,
                        ShopDescription = shop.ShopDescription,
                        ShopOwnerName = user.UserName // Assuming User has a UserName property
                    }).ToList(); ;
        }
        public async Task AddShopAsync(ShopViewModel shopViewModel)
        {
            var shop = new ShopModel
            {
                ShopName = shopViewModel.ShopName,
                ShopEmail = shopViewModel.ShopEmail,
                ShopAddress = shopViewModel.ShopAddress,
                ShopPhone = shopViewModel.ShopPhone,
                ShopDescription = shopViewModel.ShopDescription,
                ProfileImagePath = await ProcessUploadedFile(shopViewModel.ProfileImage)
            };

            await _shopRepository.AddShopAsync(shop);
        }

        public async Task UpdateShopAsync(ShopViewModel shopViewModel)
        {
            var shop = await _shopRepository.GetShopByIdAsync(shopViewModel.ShopId);

            if (shop == null) throw new Exception("Shop not found");

            if (shopViewModel.ProfileImage != null)
            {
                // Delete old image if it exists
                if (!string.IsNullOrEmpty(shop.ProfileImagePath) && shop.ProfileImagePath != "shop_default.png")
                {
                    string oldImagePath = Path.Combine(_environment.WebRootPath, "img/shop", shop.ProfileImagePath);
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }
                shop.ProfileImagePath = await ProcessUploadedFile(shopViewModel.ProfileImage);
            }

            // Update other shop details
            shop.ShopName = shopViewModel.ShopName;
            shop.ShopPhone = shopViewModel.ShopPhone;
            shop.ShopDescription = shopViewModel.ShopDescription;
            shop.ShopAddress = shopViewModel.ShopAddress;
            shop.ShopEmail = shopViewModel.ShopEmail;
            if (shop.Is_confirm == 0)
            {
                shop.Is_confirm = 1;
            }
            else
            {
                shop.Is_confirm = shop.Is_confirm;
            }
            await _shopRepository.UpdateShopAsync(shop);
        }
        public Task<int> ShopCount()
        {
            return _shopRepository.ShopCount();
        }
        public async Task DeleteShopAsync(int shopId)
        {
            await _shopRepository.DeleteShopAsync(shopId);
        }

        public async Task<DashboardViewModel> GetOwnerDashboardAsync(string userId)
        {
            var shop = await _shopRepository.GetShopByUserIdAsync(userId);
            var orders = await _shopRepository.GetOrdersByShopIdAsync(shop.ShopId);
            var items = await _shopRepository.GetItemsByShopIdAsync(shop.ShopId);
            var customerIds = orders.Select(o => o.User_Id).Distinct().ToList();
            var customers = await _shopRepository.GetUsersByIdsAsync(customerIds);

            var orderCountsByDay = orders.GroupBy(o => o.OrderDate.Date)
                                          .Select(g => new { Date = g.Key, Count = g.Count() })
                                          .OrderBy(d => d.Date)
                                          .ToList();

            var labels = orderCountsByDay.Select(d => d.Date.ToString("yyyy-MM-dd")).ToArray();
            var data = orderCountsByDay.Select(d => d.Count).ToArray();

            return new DashboardViewModel
            {
                OrderCount = orders.Count,
                ItemCount = items.Count,
                CustomerCount = customers.Count,
                OrderData = orders,
                ItemData = items,
                UserData = customers,
                Labels = labels,
                Datas = data
            };
        }

        private async Task<string> ProcessUploadedFile(IFormFile file)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "img/shop");
                Directory.CreateDirectory(uploadsFolder);
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
