using IdentityDemo.Models;
using IdentityDemo.Services;
using IdentityDemo.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityDemo.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> SendInvoiceEmail(string htmlContent)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                await _orderService.SendInvoiceEmailAsync(htmlContent, currentUser);
                TempData["test"] = "hahaha"; // Test
                ViewBag.Message = "Email sent successfully!"; // Test
                return RedirectToAction("SaveInvoice", "Order");
            }
            catch (Exception ex)
            {
                // Log the exception
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Order_confirm(Dictionary<int, int> selectedItems)
        {
            TempData["SelectedItems"] = JsonConvert.SerializeObject(selectedItems);
            if (selectedItems == null || selectedItems.Count == 0)
            {
                return RedirectToAction("HomePageItems", "Item");
            }

            var invoiceViewModel = await _orderService.PrepareInvoiceAsync(selectedItems, HttpContext);

            // Store InvoiceViewModel in TempData
            TempData["InvoiceViewModel"] = JsonConvert.SerializeObject(invoiceViewModel);

            return View(invoiceViewModel); // Pass the InvoiceViewModel to the view
        }

        public async Task<IActionResult> Invoice()
        {
            if (TempData["SelectedItems"] == null)
            {
                return RedirectToAction("Show_error_loading", "Home");
            }
            var selectedItemsJson = TempData["SelectedItems"].ToString();
            var selectedItems = JsonConvert.DeserializeObject<Dictionary<int, int>>(selectedItemsJson); // Retrieve invoice dictionary data

            var invoiceViewModel = await _orderService.PrepareInvoiceAsync(selectedItems, HttpContext);

            // Store InvoiceViewModel in TempData
            TempData["InvoiceViewModel"] = JsonConvert.SerializeObject(invoiceViewModel);

            return View(invoiceViewModel); // Pass the InvoiceViewModel to the view
        }
        public async Task<IActionResult> SaveInvoice()
        {
            try
            {
                if (TempData["InvoiceViewModel"] == null)
                {
                    // Handle case where InvoiceViewModel is null
                    return BadRequest("Invalid data received");
                }

                // Example: Save or process the invoiceViewModel data as needed
                var invoiceViewModelJson = TempData["InvoiceViewModel"].ToString();
                InvoiceViewModel invoiceViewModel = JsonConvert.DeserializeObject<InvoiceViewModel>(invoiceViewModelJson);

                await _orderService.SaveOrderAsync(invoiceViewModel);

                return RedirectToAction("HomePageItems", "Item");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Show_error_loading","Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int orderid)
        {
            await _orderService.DeleteOrderAsync(orderid);
            return RedirectToAction("Owner_order_list", "Shop");
        }
    }
}
