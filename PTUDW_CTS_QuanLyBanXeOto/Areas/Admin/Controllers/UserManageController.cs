﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTUDW_CTS_QuanLyBanXeOto.Areas.Admin.Models;
using PTUDW_CTS_QuanLyBanXeOto.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PTUDW_CTS_QuanLyBanXeOto.Areas.Admin.Controllers
{
    //Tạo controller cho phần quản lý người dùng trong Admin
    [Area("Admin")]
    public class UserManageController : BaseController
    {
        private readonly ILogger<UserManageController> _logger;
        private readonly DataContext _context;

        public UserManageController(ILogger<UserManageController> logger, DataContext dataContext) : base()
        {
            _logger = logger;
            _context = dataContext;
        }
        //Method trả về view quản lý người dùng
        public IActionResult UserManage()
        {
            //Lấy dữ liệu từ sql
            var ctmlist = (from ctm in _context.User
                           where ctm.TypeID == 2
                           orderby ctm.UserID
                           select ctm).ToList();
            return View(ctmlist);
        }

        [HttpGet]
        public IActionResult Delete(long? id)
        {
            var delUser = _context.User.Find(id);
            return View("Delete", delUser);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(User _user)
        {
            var delUser = _context.User.Find(_user.UserID);
            _context.User.Remove(delUser);
            await _context.SaveChangesAsync();
            TempData["alertMessage"] = "Xóa người dùng thành công!";
            return RedirectToAction("UserManage", "UserManage");
        }

        [HttpGet]
        public IActionResult Edit(long? id)
        {
            var editUser = _context.User.Find(id);
            return View("Edit", editUser);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(User _u)
        {
            var userduplicate = _context.User
                .Where(u => u.Username.Equals(_u.Username) && u.UserID != _u.UserID)
                .ToList();

            var emailduplicate = _context.User
                .Where(u => u.Email.Equals(_u.Email) && u.UserID != _u.UserID)
                .ToList();

            var phoneduplicate = _context.User
                .Where(u => u.SoDienThoai.Equals(_u.SoDienThoai) && u.UserID != _u.UserID)
                .ToList();

            var CMNDduplicate = _context.User
                .Where(u => u.CMND.Equals(_u.CMND) && u.UserID != _u.UserID)
                .ToList();

            if (userduplicate.Count() > 0)
            {
                TempData["alertMessage"] = "Tên đăng nhập đã tồn tại!";
                return RedirectToAction("Edit", "UserManage", new { id = _u.UserID });
            }
            else if (emailduplicate.Count() > 0)
            {
                TempData["alertMessage"] = "Email đã tồn tại!";
                return RedirectToAction("Edit", "UserManage", new { id = _u.UserID });
            }
            else if (phoneduplicate.Count() > 0)
            {
                TempData["alertMessage"] = "Số điện thoại đã tồn tại!";
                return RedirectToAction("Edit", "UserManage", new { id = _u.UserID });
            }
            else if (CMNDduplicate.Count() > 0)
            {
                TempData["alertMessage"] = "Số CMND/CCCD đã tồn tại!";
                return RedirectToAction("Edit", "UserManage", new { id = _u.UserID });
            }
            else
            {
                var editUser = new User
                {
                    UserID = _u.UserID,
                    CMND = _u.CMND,
                    HoTen = _u.HoTen,
                    DiaChi = _u.DiaChi,
                    NamSinh = _u.NamSinh,
                    Email = _u.Email,
                    SoDienThoai = _u.SoDienThoai,
                    TypeID = _u.TypeID,
                    UserImage = _u.UserImage,
                    Username = _u.Username,
                    Password = _u.Password,
                    IsActive = _u.IsActive
                };
                _context.User.Update(editUser);
                await _context.SaveChangesAsync();
                TempData["alertMessage"] = "Cập nhật người dùng thành công!";
                return RedirectToAction("UserManage", "UserManage");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
