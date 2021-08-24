namespace tabuleiro
{
    class Peca
    {
        public Posicao posicao { get; set; }
        public Cor cor { get; protected set; }
        public int qTMovimentos { get; protected set; }
        public Tabuleiro tabuleiro { get; protected set; }

        public Peca(Tabuleiro tabuleiro, Cor cor)
        {
            this.tabuleiro = tabuleiro;
            this.cor = cor;
            this.posicao = null;
            this.qTMovimentos = 0;
        }
    }
}
