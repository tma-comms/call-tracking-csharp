﻿using System.Net;
using System.Web.Mvc;
using CallTracking.Web.Domain.Twilio;
using CallTracking.Web.Models;
using CallTracking.Web.Models.Repository;
using HttpStatusCodeResult = System.Web.Mvc.HttpStatusCodeResult;

namespace CallTracking.Web.Controllers
{
    public class LeadSourcesController : Controller
    {
        private readonly IRepository<LeadSource> _repository;
        private readonly IRestClient _restClient;

        public LeadSourcesController()
            : this(new LeadSourcesRepository(), new RestClient()) { }

        public LeadSourcesController(IRepository<LeadSource> repository, IRestClient restClient)
        {
            _repository = repository;
            _restClient = restClient;
        }

        // POST: LeadSources/Create
        [HttpPost]
        public ActionResult Create(string phoneNumber)
        {
            var twilMLApplicationSid = Credentials.TwiMLApplicationSid ?? _restClient.GetApplicationSid();
            var twilioNumber = _restClient.PurchasePhoneNumber(phoneNumber, twilMLApplicationSid);
            var leadSource = new LeadSource
            {
                IncomingNumberNational = twilioNumber.FriendlyName,
                IncomingNumberInternational = twilioNumber.PhoneNumber
            };

            _repository.Create(leadSource);

            return RedirectToAction("Edit", new {id = leadSource.Id});
        }

        // GET: LeadSources/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var leadSource = _repository.Find(id.Value);
            if (leadSource == null)
            {
                return HttpNotFound();
            }

            return View(leadSource);
        }

        // POST: LeadSources/Edit/
        [HttpPost]
        public ActionResult Edit(
            [Bind(Include = "Id,Name,IncomingNumberNational,IncomingNumberInternational,ForwardingNumber")]
            LeadSource leadSource)
        {
            if (ModelState.IsValid)
            {
                _repository.Update(leadSource);
                return RedirectToAction("Index", new { Controller = "Dashboard" });
            }

            return View(leadSource);
        }
    }
}