using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GuessingGame.Core.Domain.Image;
using GuessingGame.Core.Domain.Player;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.AI;
using MediatR;


namespace GuessingGame.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        private readonly IMediator _mediator;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }
        public DbSet<Image> Images { get; set; } = null!;
        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<AIData> AI { get; set; } = null!;

    }
}
