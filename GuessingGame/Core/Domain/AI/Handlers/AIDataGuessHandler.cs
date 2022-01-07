using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using GuessingGame.Data;
using GuessingGame.Core.Domain.Game.Pipelines;
using GuessingGame.Core.Domain.Player;
using GuessingGame.Core.Domain.AI;
using GuessingGame.Core.Domain.AI.Events;
using GuessingGame.Core.Domain.AI.Pipelines;

namespace GuessingGame.Core.Domain.AI.Handlers
{
    public class AIDataGuessHandler : INotificationHandler<AIDataGuess>
    {
        private readonly ApplicationDbContext _db;
        private readonly IMediator _mediator;

        public AIDataGuessHandler(ApplicationDbContext db, IMediator mediator){
			_db = db ?? throw new System.ArgumentNullException(nameof(db));
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }
        
        public async Task Handle(AIDataGuess notification, CancellationToken cancellationToken)
		{ // This method checks wether a guess is true or false, and thereafter adds each guess to a list of guesses. 
            // A guess holds the guess, the tiles available at that point, and a boolean which is set to either true or false, based on if the guess the user sent 
            // is correct
            var CurrentAI = await _mediator.Send(new GetAIDataByGameId.Request(notification.GameId));
            if (notification.CorrectAnswer){
                var guess = new Guess(notification.Guess, notification.UsedTiles, notification.CorrectAnswer);
                CurrentAI.Guesses.Add(guess);
                CurrentAI.Attempts++;
                CurrentAI.Correct = true;
                _db.Update(CurrentAI);
                await _db.SaveChangesAsync(cancellationToken);
                //Bruker den i tillegg til inkrementering av tilesa som blir valgt.
                //Kan brukes til å se om det er en forskjell mellom det brukerne sier hadde vært best, og hva som egentlig er best
            } else {
                var guess = new Guess(notification.Guess, notification.UsedTiles, notification.CorrectAnswer);
                CurrentAI.Guesses.Add(guess);
                CurrentAI.Attempts++;
                await _db.SaveChangesAsync(cancellationToken);
            }
            await _db.SaveChangesAsync(cancellationToken);
		}

    }

}