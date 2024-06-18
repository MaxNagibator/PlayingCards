﻿

namespace PlayingCards.Durak.Web.Models
{
    public class GetStatusModel
    {
        public TableModel? Table { get; set; }

        public TableModel[]? Tables { get; set; }

        public class TableModel
        {
            public Guid Id { get; set; }

            public int DeckCardsCount { get; set; }

            public CardModel[]? MyCards { get; set; }

            public CardModel? Trump { get; set; }

            public TableCardModel[]? Cards { get; set; }

            public PlayerModel[] Players { get; set; }
        }

        public class CardModel
        {
            public CardModel(Card card)
            {
                Rank = card.Rank.Value;
                Suit = card.Suit.Value;
            }

            public int Rank { get; set; }

            public int Suit { get; set; }
        }

        public class TableCardModel
        {
            public CardModel? AttackCard { get; set; }

            public CardModel? DefenceCard { get; set; }
        }

        public class PlayerModel
        {
            public string Name { get; set; }

            public int CardsCount { get; set; }
        }
    }
}
