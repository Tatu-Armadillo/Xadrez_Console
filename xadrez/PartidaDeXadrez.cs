using System.Collections.Generic;
using tabuleiro;

namespace xadrez
{
    public class PartidaDeXadrez
    {
        public Tabuleiro tab { get; private set; }
        public int turno { get; private set; }
        public Cor jogadorAtual { get; private set; }
        public bool terminada { get; private set; }
        private HashSet<Peca> pecas;
        private HashSet<Peca> capturadas;
        public bool xeque { get; private set; }

        public PartidaDeXadrez()
        {
            tab = new Tabuleiro(8, 8);
            turno = 1;
            jogadorAtual = Cor.Branca;
            terminada = false;
            pecas = new HashSet<Peca>();
            capturadas = new HashSet<Peca>();
            xeque = false;
            colocarPecas();
        }

        public Peca executarMovimento(Posicao origem, Posicao destino)
        {
            Peca p = tab.retirarPeca(origem);
            p.incrementarQtdMovimentos();
            Peca pecaCapturada = tab.retirarPeca(destino);
            tab.colocarPeca(p, destino);
            if (pecaCapturada != null)
            {
                capturadas.Add(pecaCapturada);
            }
            return pecaCapturada;
        }

        public void desfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = tab.retirarPeca(destino);
            p.decrementarQtdMovimentos();
            if (pecaCapturada != null)
            {
                tab.colocarPeca(pecaCapturada, destino);
                capturadas.Remove(pecaCapturada);
            }
            tab.colocarPeca(p, origem);
        }

        public void realizaJogada(Posicao origem, Posicao destino)
        {
            Peca pecaCapturada = executarMovimento(origem, destino);
            if (estaEmXeque(jogadorAtual))
            {
                desfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em xeque!");
            }

            if (estaEmXeque(adversaria(jogadorAtual)))
            {
                xeque = true;
            }
            else
            {
                xeque = false;
            }
            if (testeXequemate(adversaria(jogadorAtual)))
            {
                terminada = true;
            }
            turno++;
            mudaJogador();
        }

        public void validarPosicaoOrigem(Posicao pos)
        {
            if (tab.peca(pos) == null)
            {
                throw new TabuleiroException("Não existe peça na posição de origem escolhida!");
            }
            if (jogadorAtual != tab.peca(pos).cor)
            {
                throw new TabuleiroException("A peça de origem não é sua!");
            }
            if (!tab.peca(pos).existeMovimentosPossiveis())
            {
                throw new TabuleiroException("Não há movimentos possiveis para a peça de origem escolhida!");
            }
        }

        public void validarPosicaoDestino(Posicao origem, Posicao destino)
        {
            if (!tab.peca(origem).movimentoPossivel(destino))
            {
                throw new TabuleiroException("Posição de destino invalida!");
            }
        }

        private void mudaJogador()
        {
            if (jogadorAtual == Cor.Branca)
            {
                jogadorAtual = Cor.Preta;
            }
            else
            {
                jogadorAtual = Cor.Branca;
            }
        }

        public HashSet<Peca> pecasCapturadas(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in capturadas)
            {
                if (x.cor == cor)
                {
                    aux.Add(x);
                }
            }
            return aux;
        }

        public HashSet<Peca> pecasEmJogo(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in pecas)
            {
                if (x.cor == cor)
                {
                    aux.Add(x);
                }
            }
            aux.ExceptWith(pecasCapturadas(cor));
            return aux;
        }

        private Cor adversaria(Cor cor)
        {
            if (cor == Cor.Branca)
            {
                return Cor.Preta;
            }
            else
            {
                return Cor.Branca;
            }
        }

        private Peca rei(Cor cor)
        {
            foreach (Peca x in pecasEmJogo(cor))
            {
                if (x is Rei)
                {
                    return x;
                }
            }
            return null;
        }

        public bool estaEmXeque(Cor cor)
        {
            Peca R = rei(cor);
            if (R == null)
            {
                throw new TabuleiroException("Não tem rei da cor " + cor + " no tabuleiro");
            }
            foreach (Peca x in pecasEmJogo(adversaria(cor)))
            {
                bool[,] mat = x.movimentosPossiveis();
                if (mat[R.posicao.linha, R.posicao.coluna])
                {
                    return true;
                }
            }
            return false;
        }

        public bool testeXequemate(Cor cor)
        {
            if (!estaEmXeque(cor))
            {
                return false;
            }
            foreach (Peca x in pecasEmJogo(cor))
            {
                bool[,] mat = x.movimentosPossiveis();
                for (int i = 0; i < tab.linhas; i++)
                {
                    for (int j = 0; j < tab.colunas; j++)
                    {
                        if (mat[i, j])
                        {
                            Posicao origem = x.posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = executarMovimento(origem, destino);
                            bool testeXequemate = estaEmXeque(cor);
                            desfazMovimento(origem, destino, pecaCapturada);
                            if (!testeXequemate)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public void colocarNovaPeca(char coluna, int linha, Peca peca)
        {
            tab.colocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
            pecas.Add(peca);
        }

        private void colocarPecas()
        {
            // Peças Brancas
            colocarNovaPeca('A', 2, new Peao(tab, Cor.Branca));
            colocarNovaPeca('B', 2, new Peao(tab, Cor.Branca));
            colocarNovaPeca('C', 2, new Peao(tab, Cor.Branca));
            colocarNovaPeca('D', 2, new Peao(tab, Cor.Branca));
            colocarNovaPeca('E', 2, new Peao(tab, Cor.Branca));
            colocarNovaPeca('F', 2, new Peao(tab, Cor.Branca));
            colocarNovaPeca('G', 2, new Peao(tab, Cor.Branca));
            colocarNovaPeca('H', 2, new Peao(tab, Cor.Branca));
            colocarNovaPeca('H', 1, new Torre(tab, Cor.Branca));
            colocarNovaPeca('A', 1, new Torre(tab, Cor.Branca));
            colocarNovaPeca('B', 1, new Cavalo(tab, Cor.Branca));
            colocarNovaPeca('G', 1, new Cavalo(tab, Cor.Branca));
            colocarNovaPeca('C', 1, new Bispo(tab, Cor.Branca));
            colocarNovaPeca('F', 1, new Bispo(tab, Cor.Branca));
            colocarNovaPeca('E', 1, new Rainha(tab, Cor.Branca));
            colocarNovaPeca('D', 1, new Rei(tab, Cor.Branca));

            // Peças Brancas
            colocarNovaPeca('A', 7, new Peao(tab, Cor.Preta));
            colocarNovaPeca('B', 7, new Peao(tab, Cor.Preta));
            colocarNovaPeca('C', 7, new Peao(tab, Cor.Preta));
            colocarNovaPeca('D', 7, new Peao(tab, Cor.Preta));
            colocarNovaPeca('E', 7, new Peao(tab, Cor.Preta));
            colocarNovaPeca('F', 7, new Peao(tab, Cor.Preta));
            colocarNovaPeca('G', 7, new Peao(tab, Cor.Preta));
            colocarNovaPeca('H', 7, new Peao(tab, Cor.Preta));
            colocarNovaPeca('H', 8, new Torre(tab, Cor.Preta));
            colocarNovaPeca('A', 8, new Torre(tab, Cor.Preta));
            colocarNovaPeca('B', 8, new Cavalo(tab, Cor.Preta));
            colocarNovaPeca('G', 8, new Cavalo(tab, Cor.Preta));
            colocarNovaPeca('C', 8, new Bispo(tab, Cor.Preta));
            colocarNovaPeca('F', 8, new Bispo(tab, Cor.Preta));
            colocarNovaPeca('E', 8, new Rainha(tab, Cor.Preta));
            colocarNovaPeca('D', 8, new Rei(tab, Cor.Preta));
        }
    }
}