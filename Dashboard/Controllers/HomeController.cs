﻿using Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Dashboard.Backoffice;
using Dashboard.Backoffice.Models;

namespace Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Dictionary<int, Modem> _modems;
        

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _modems = ModemManager.Modems;
        }

        public IActionResult Index()
        {
            MainPageModel model = new MainPageModel();
            GetMessages(model);
            return View(model);
        }

        public IActionResult UpdateExternalIp(int id)
        {
            if(_modems.ContainsKey(id)) _ = ExternalIpMonitor.UpdateExternalIpAsync(modem: _modems[id]);
            return RedirectToAction("Index");
        }
        public IActionResult Reboot (int id)
        {
            if (_modems.ContainsKey(id))
            {
                _ = ModemManager.RebootModemAsync(id);
                MessageCenter.Warnings.Add($"Запрос на перезагрузку модема {_modems[id].Host} принят.");
            }
            else
            {
                MessageCenter.Errors.Add($"Модем с ID {id} не найден.");
            }
            return RedirectToAction("Index");
        }
       
        public IActionResult Logs(int id)
        {
            if (!_modems.ContainsKey(id) | id == 0) return View();
            return View(_modems[id].Logs);
        }

        public IActionResult Edit(int id)
        {
            Modem modem =  new Modem();
            if (!_modems.ContainsKey(id) | id == 0)
            {
                ViewData["Title"] = "Add New Proxy";
            }
            else
            {
                ViewData["Title"] = $"Edit Proxy {id}";
                modem = _modems[id];
            }
                 
            return View(modem);
        }

        [HttpPost]
        public IActionResult Edit(Modem modem)
        {
            if (!_modems.ContainsKey(modem.Port))
            {
                //добавляем модем
                _modems.Add(modem.Port, modem);
                MessageCenter.Info.Add("Модем был успешно добавлен.");
            }
            else if (_modems.ContainsKey(modem.Port))
            { 
                _modems[modem.Port].CopyFrom(modem);
                MessageCenter.Info.Add("Сведения о модеме были успешно обновлены.");
            }
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void GetMessages(MainPageModel model)
        {
            if (MessageCenter.Errors.Any())
            {
                model.Errors.AddRange(MessageCenter.Errors);
                MessageCenter.Errors.Clear();
            }

            if (MessageCenter.Warnings.Any())
            {
               model.Warnings.AddRange(MessageCenter.Warnings);
               MessageCenter.Warnings.Clear();
            }

            if (MessageCenter.Info.Any())
            {
                model.Info.AddRange(MessageCenter.Info);
                MessageCenter.Info.Clear();
            }
            
        }
    }
}