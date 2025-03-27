# Projeto de Desempenho e Concorrência com `BlockingCollection` em C#

Este projeto foi criado para testar e comparar o desempenho do modelo sequencial versus o uso da **`BlockingCollection`** em C#. A comparação foi feita ao realizar requisições HTTP em uma API utilizando dados gerados de veículos, de forma sequencial e concorrente.

## Requisitos

1. **Python**: Para gerar o arquivo de veículos.
2. **Conta no SerpApi**: Para testar as requisições de busca de veículos.

### Passo 1: Gerar o arquivo `veiculos.json` com o Python

1. Navegue até a pasta onde o arquivo **`gerador.py`** está localizado.
2. Execute o script **`gerador.py`** para gerar o arquivo JSON com dados de veículos. Esse arquivo será utilizado pelo código em C#.

```bash
python gerador.py