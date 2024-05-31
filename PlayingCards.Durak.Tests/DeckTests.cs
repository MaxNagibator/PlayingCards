namespace PlayingCards.Durak.Tests
{
    public class DeckTests
    {
        /// <summary>
        /// ������������ ������ � ��������� ��� � ��� 9 ��������� ���� � 4 �����, � ���� 36 ����.
        /// </summary>
        [Test]
        public void DeckCardsCountTest()
        {
            var deck = new Deck(new RandomDeckCardGenerator());
            deck.Shuffle();
            var cards = new List<Card>();
            while (deck.CardsCount > 0)
            {
                var card = deck.PullCard();
                cards.Add(card);
            }
            Assert.That(cards.Count, Is.EqualTo(36));
            Assert.That(cards.GroupBy(x => x.Rank.Value).Count(), Is.EqualTo(9));
            Assert.That(cards.GroupBy(x => x.Suit.Value).Count(), Is.EqualTo(4));
        }

        /// <summary>
        /// ������� ������� �� 6 ���� � ������ ����.
        /// </summary>
        /// <param name="playerCount">���������� �������.</param>
        [Test]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void TwoPlayersStartGameTest(int playerCount)
        {
            var game = new Game();
            for (var i = 0; i < playerCount; i++)
            {
                game.AddPlayer("player" + i);
            }
            game.InitCardDeck();

            foreach (var player in game.Players)
            {
                Assert.That(player.Hand.Cards.Count(), Is.EqualTo(6));
            }
        }

        /// <summary>
        /// ��������, ��� ������ �����.
        /// </summary>
        /// <remarks>
        /// � ������� �������� ������ ���� ������ ����������� �������� �� ����, ��� � ������.
        /// </remarks>
        /// <param name="playerCount">���������� ������� � ����.</param>
        [Test]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void ActivePlayerTest(int playerCount)
        {
            var game = new Game();
            for (var i = 0; i < playerCount; i++)
            {
                game.AddPlayer("Player" + i);
            }
            game.InitCardDeck();

            Assert.IsNotNull(game.ActivePlayer);
            var activePlayerMinSuitCard = game.ActivePlayer.Hand.GetMinSuitCard(game.Deck.TrumpCard.Suit);
            Assert.IsNotNull(activePlayerMinSuitCard);

            foreach (var player in game.Players)
            {
                if (player.Name != game.ActivePlayer.Name)
                {
                    var card = player.Hand.GetMinSuitCard(game.Deck.TrumpCard.Suit);
                    var cardRank = card?.Rank.Value ?? Int32.MaxValue;
                    Assert.GreaterOrEqual(cardRank, activePlayerMinSuitCard.Rank.Value);
                }
            }
        }

        /// <summary>
        /// ��������, �� ����, ������ �����.
        /// </summary>
        /// <remarks>
        /// ��������� ����� ���������, ����������.
        /// </remarks>
        /// <param name="playerCount">���������� ������� � ����.</param>
        [Test]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void DefencePlayerTest(int playerCount)
        {
            var game = new Game();
            for (var i = 0; i < playerCount; i++)
            {
                game.AddPlayer("Player" + i);
            }
            game.InitCardDeck();

            var activePlayerNumber = game.ActivePlayer.Name.Substring("Player".Length);
            var defencePlayerNumber = Convert.ToInt32(activePlayerNumber) + 1;
            if (defencePlayerNumber >= playerCount)
            {
                defencePlayerNumber = 0;
            }
            var defencePlayerName = "Player" + defencePlayerNumber;
            Assert.IsNotNull(game.DefencePlayer);
            Assert.That(game.DefencePlayer.Name, Is.EqualTo(defencePlayerName));
        }

        /// <summary>
        /// ���������� �� �����.
        /// </summary>
        [Test]
        [TestCase(0, 1, 8, true, TestName = "���� �������� �������, �������� �������")]
        [TestCase(2, 1, 8, false, TestName = "���� �������� ��������, �������� �������")]
        [TestCase(0, 10, 8, false, TestName = "���� �������� �������, ���������� �������")]
        [TestCase(0, 1, 9, true, TestName = "���� ���������� �������, ���������� ������� ��� �� �����")]
        [TestCase(2, 1, 9, false, TestName = "���� ���������� ��������, ���������� ������� ��� �� �����")]
        [TestCase(9, 1, 8, true, TestName = "���� ���������� �������, �������� �������")]
        [TestCase(9, 1, 34, false, TestName = "���� ���������� �������, ���������� ������� ������ �����")]
        public void SuccessDefenceTrumpSuitTest(
            int attackCardIndex,
            int defenceCardIndex,
            int trumCardIndex,
            bool isSuccess)
        {
            var attackCard = CardsHolder.GetCards()[attackCardIndex];
            var defenceCard = CardsHolder.GetCards()[defenceCardIndex];
            var trumpCard = CardsHolder.GetCards()[trumCardIndex];
            var game = new Game();
            game.Deck = new Deck(new EmptyDeckCardGenerator()) { TrumpCard = trumpCard };
            var attackTableCard = new TableCard(game, attackCard);
            if (isSuccess)
            {
                attackTableCard.Defence(defenceCard);
            }
            else
            {
                Assert.Throws<Exception>(() => attackTableCard.Defence(defenceCard));
            }
        }

        /// <summary>
        /// ������� ���� ����� � ������ �.
        /// </summary>
        /// <remarks>
        /// ���������, ��� ����� ������� ������, ��� ������� ����� �� ���� �� 6.
        /// ���������, ��� � ������ ����� �� 2 ����� ������.
        /// </remarks>
        [Test]
        public void PlayOneRoundOneCardDefenceTest()
        {
            var game = new Game();
            game.AddPlayer("1");
            game.AddPlayer("2");
            var player1 = game.Players[0];
            var player2 = game.Players[1];
            game.Deck = new Deck(new NotSortedDeckCardGenerator());
            game.InitCardDeck();
            // ����� ������� �����
            player1.Hand.Attack(1);
            // ���������� ������� ������
            player2.Hand.Defence(0, 0);
            game.StopRound();
            Assert.That(player1.Hand.Cards.Count, Is.EqualTo(6));
            Assert.That(player2.Hand.Cards.Count, Is.EqualTo(6));
            Assert.That(game.Deck.CardsCount, Is.EqualTo(36 - 6 - 6 - 2));
        }

        /// <summary>
        /// ������� ��� �����, ���� ������, � ������ �� ������, � �������� �� �� ����.
        /// </summary>
        /// <remarks>
        /// ���������, ��� ��������� ������ ����� �� 6.
        /// ���������, ��� ������������ ������ ���� ��� ����� �����������, � � ���� ������ �� 8.
        /// ���������, ��� � ������ ����� �� 2 ����� ������.
        /// </remarks>
        [Test]
        public void PlayOneRoundTwoCardAndNotDefenceTest()
        {
            var game = new Game();
            game.AddPlayer("1");
            game.AddPlayer("2");
            var player1 = game.Players[0];
            var player2 = game.Players[1];
            game.Deck = new Deck(new NotSortedDeckCardGenerator());
            game.InitCardDeck();
            // ����� ������� �����
            player1.Hand.Attack(0);
            // ����� ������� �����
            player1.Hand.Attack(0);
            // ���������� ������� ������ �� ����
            player2.Hand.Defence(0, 1);
            // � ���� �� ������ �� �����, �������� �� ����
            game.StopRound();
            Assert.That(player1.Hand.Cards.Count, Is.EqualTo(6));
            Assert.That(player2.Hand.Cards.Count, Is.EqualTo(8));
            Assert.That(game.Deck.CardsCount, Is.EqualTo(36 - 6 - 6 - 2));
        }
    }
}