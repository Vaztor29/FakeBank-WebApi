﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FakeBank.Api.Models;
using FakeBank.Controllers;
using FakeBank.Data.Business.Services;
using FakeBank.Data.Entities;

namespace FakeBank.Api.Controllers
{
    [RoutePrefix("api/Transaction")]
    public class TransactionController : BaseAPIController
    {
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult SaveTransaction(TransactionBindingModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var tokenService = new TokenService();
            var accountService = new AccountService();
            var cardService = new CardService();
            var transactionService = new TransactionService();

            var token = tokenService.GetByToken(model.Token);
            if (token == null) return BadRequest("Token invalido.");

            var card = cardService.GetByCardNumber(model.CardNumber);
            if (card == null) return BadRequest("El Numbero de Tarjeta no existe.");
            if (card.ExpirationDate != model.ExpirationDate) return BadRequest();
            if (card.SecurityCode != model.SecurityCode) return BadRequest();

            
            var accountSource = accountService.GetByCardNumber(model.CardNumber);
            var accountDestination = accountService.GetByToken(model.Token);

            if (model.Amount > accountSource.Balance)
                return BadRequest("La cuenta no tiene suficiente saldo.");
            var modified = accountService.UpdateBalance(accountSource, accountDestination, model.Amount);
            if (!modified) return InternalServerError();

            var saved = transactionService.Save(new Transaction
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Now,
                IdSourceAccount = accountSource.Id,
                IdDestinationAccount = accountDestination.Id,
                Amount = model.Amount,
                Type = (int) TransactionType.ApiTransaction
            });
            if (!saved) return InternalServerError();
            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "employee,moralperson,physicallperson")]
        [Route("Transfer")]
        public IHttpActionResult Transfer(TransferBindingModel model)
        {
            var cardService = new CardService();
            var accountService = new AccountService();
            var transactionService = new TransactionService();

            var card = cardService.GetByCardNumber(model.CardNumber);
            if (card == null) BadRequest("Numero de Tarjeta Invalido.");
            var accountSource = accountService.GetById(model.IdAccount);
            var accountDestination = accountService.GetByCardNumber(model.CardNumber);
            var modified = accountService.UpdateBalance(accountSource, accountDestination, model.Amount);
            if (!modified) return InternalServerError();
            var saved = transactionService.Save(new Transaction
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Now,
                IdSourceAccount = accountSource.Id,
                IdDestinationAccount = accountDestination.Id,
                Amount = model.Amount,
                Type = (int)TransactionType.Transaction
            });
            if (!saved) return InternalServerError();
            return Ok();
        }

        [HttpGet]
        [Authorize(Roles= "employee,moralperson,physicallperson")]
        [Route("GetByUser/{Id}")]
        public IHttpActionResult GetTransactionByUserId(string id)
        {
            var transactionService = new TransactionService();
            var transactions = transactionService.GetByUserId(id);
            return (transactions != null) ? (IHttpActionResult) Ok(TheModelFactory.Create(transactions)) : NotFound();
        }
    }
}