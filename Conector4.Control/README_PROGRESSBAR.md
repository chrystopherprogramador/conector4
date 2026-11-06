# SegmentedProgressBar - Controle de Barra de Progresso Segmentada para Windows Forms

## ?? Descrição

Controle de barra de progresso segmentada **NATIVO para Windows Forms** com:
- ? 30 segmentos individuais (configurável)
- ? Gradiente azul nos segmentos preenchidos
- ? Segmentos vazios em cinza escuro
- ? Borda dupla (cinza externa)
- ? Fundo preto
- ? Porcentagem exibida em branco (negrito)
- ? **Aparece automaticamente na Toolbox do Visual Studio**

## ?? Características Visuais

- **Cores dos segmentos preenchidos**: Gradiente azul (RGB: 100,180,255 ? 0,120,215 ? 0,80,180)
- **Cores dos segmentos vazios**: Cinza escuro (RGB: 60,60,60)
- **Fundo**: Preto
- **Borda**: Cinza com 3px
- **Texto**: Branco, negrito, tamanho 40% da altura
- **100% Windows Forms nativo - Sem WPF ou ElementHost**

## ?? Como Usar

### 1. Uso no Designer (RECOMENDADO)

Após compilar o projeto, o controle aparece automaticamente na Toolbox:

1. **Compile** o projeto `Conector4`
2. Abra seu formulário no **Designer**
3. Procure por `SegmentedProgressBar` na **Toolbox** (seção Conector4.Controls)
4. **Arraste** para o formulário
5. Configure as propriedades na janela **Properties**:
   - `Value`: 0 a 100 (valor atual)
   - `Maximum`: valor máximo
   - `SegmentCount`: número de segmentos (padrão 30)
   - `ShowPercentage`: true/false

### 2. Uso em Código C#

```csharp
using Conector4.Controls;

// Criar a barra de progresso
var progressBar = new SegmentedProgressBar
{
    Location = new Point(20, 20),
    Width = 600,
    Height = 50,
    Value = 78,
    Maximum = 100,
    SegmentCount = 30,
    ShowPercentage = true
};

// Adicionar ao formulário
this.Controls.Add(progressBar);

// Atualizar o valor
progressBar.Value = 50; // Atualiza para 50%
```

### 3. Uso em Formulário de Importação (importProduts.cs)

```csharp
using Conector4.Controls;

public partial class ImportProduts : Form
{
    private SegmentedProgressBar progressBar;

    public ImportProduts()
    {
        InitializeComponent();
        
        // Criar a barra de progresso
        progressBar = new SegmentedProgressBar
        {
            Dock = DockStyle.Top,
            Height = 60,
            Value = 0,
            Maximum = 100,
            SegmentCount = 30,
            ShowPercentage = true
        };
        
        // Adicionar ao topo do formulário
        this.Controls.Add(progressBar);
        progressBar.BringToFront();
    }

    private async void btnImportar_Click(object sender, EventArgs e)
    {
        progressBar.Value = 0;
        
        // Importar produtos
        var produtos = await ObterProdutos();
        progressBar.Maximum = produtos.Count;
        
        for (int i = 0; i < produtos.Count; i++)
        {
            await ImportarProduto(produtos[i]);
            progressBar.Value = i + 1;
            Application.DoEvents();
        }
        
        MessageBox.Show("Importação concluída!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
```

### 4. Integração com Processamento de Produtos

```csharp
using Conector4.Controls;

public partial class ProcessarProdutos : Form
{
    private SegmentedProgressBar progressBar;
    private Label lblStatus;

    private void InicializarProgressBar()
    {
        // Criar painel para organizar
        var panel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80,
            Padding = new Padding(10)
        };
        
        // Criar barra de progresso
        progressBar = new SegmentedProgressBar
        {
            Dock = DockStyle.Fill,
            Value = 0,
            Maximum = 100
        };
        
        panel.Controls.Add(progressBar);
        this.Controls.Add(panel);
    }

    private async Task ProcessarProdutosAsync()
    {
        int total = 1000;
        progressBar.Maximum = total;
        progressBar.Value = 0;
        
        for (int i = 0; i < total; i++)
        {
            await ProcessarProduto(i);
            progressBar.Value = i + 1;
            
            // Atualizar status
            lblStatus.Text = $"Processando produto {i + 1} de {total}";
            Application.DoEvents();
        }
        
        MessageBox.Show("Processamento concluído!", "Sucesso");
    }
}
```

## ?? Propriedades

| Propriedade | Tipo | Padrão | Categoria | Descrição |
|------------|------|--------|-----------|-----------|
| `Value` | double | 0.0 | Behavior | Valor atual do progresso |
| `Maximum` | double | 100.0 | Behavior | Valor máximo |
| `SegmentCount` | int | 30 | Appearance | Número de segmentos |
| `ShowPercentage` | bool | true | Appearance | Exibir texto de porcentagem |

## ?? Exemplos de Uso

### Exemplo 1: Progresso de Download
```csharp
// Inicializar
progressBar.Value = 0;
progressBar.Maximum = totalBytes;

// Durante o download
void OnDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
{
    progressBar.Value = e.BytesReceived;
    lblStatus.Text = $"Download: {e.ProgressPercentage}%";
    Application.DoEvents();
}
```

### Exemplo 2: Progresso de Processamento em Lote
```csharp
private void ProcessarLote()
{
    var produtos = GetProdutos();
    progressBar.Maximum = produtos.Count;
    progressBar.Value = 0;

    foreach (var produto in produtos)
    {
        ProcessarProduto(produto);
        progressBar.Value++;
        Application.DoEvents();
    }
    
    MessageBox.Show("Processamento concluído!");
}
```

### Exemplo 3: Progresso Assíncrono com Timer
```csharp
private Timer timer;
private int currentValue = 0;

private void IniciarProcessamento()
{
    timer = new Timer();
    timer.Interval = 100;
    timer.Tick += (s, e) =>
    {
        if (currentValue < 100)
        {
            currentValue++;
            progressBar.Value = currentValue;
        }
        else
        {
            timer.Stop();
            MessageBox.Show("Concluído!");
        }
    };
    timer.Start();
}
```

### Exemplo 4: Customização Visual
```csharp
// Barra com menos segmentos (mais largos)
var progressBar1 = new SegmentedProgressBar
{
    SegmentCount = 20,
    Value = 75,
    Height = 60,
    Width = 800,
    Dock = DockStyle.Top
};

// Barra sem porcentagem
var progressBar2 = new SegmentedProgressBar
{
    ShowPercentage = false,
    Value = 50,
    Height = 40,
    Dock = DockStyle.Bottom
};

// Barra compacta
var progressBar3 = new SegmentedProgressBar
{
    Height = 30,
    Width = 400,
    SegmentCount = 20,
    Location = new Point(10, 10)
};
```

### Exemplo 5: Integração com RabbitMQ
```csharp
using Conector4.Controls;

private SegmentedProgressBar progressBar;

private void Form_Load(object sender, EventArgs e)
{
    progressBar = new SegmentedProgressBar
    {
        Dock = DockStyle.Top,
        Height = 60
    };
    
    panelProgress.Controls.Add(progressBar);
}

private async Task ProcessarFilaAsync()
{
    var mensagens = await ObterMensagensFila();
    progressBar.Maximum = mensagens.Count;
    progressBar.Value = 0;
    
    for (int i = 0; i < mensagens.Count; i++)
    {
        await ProcessarMensagem(mensagens[i]);
        progressBar.Value = i + 1;
        
        lblStatus.Text = $"Processando mensagem {i + 1} de {mensagens.Count}";
        Application.DoEvents();
    }
}
```

## ?? Casos de Uso

1. **Importação de Dados**: Exibir progresso de importação de produtos da API Presence
2. **Sincronização**: Mostrar status de sincronização com API
3. **Processamento em Lote**: Indicar progresso de processamento de lotes
4. **Upload/Download**: Visualizar transferência de arquivos
5. **Carregamento**: Indicar carregamento de recursos
6. **Processamento de Filas**: Mostrar progresso de mensagens RabbitMQ
7. **Migração de Dados**: Exibir progresso de migração entre bancos
8. **Backup/Restore**: Visualizar progresso de operações de banco de dados

## ?? Arquivos do Projeto

- `Conector4\Controls\SegmentedProgressBar.cs` - Controle principal para Windows Forms
- `README_PROGRESSBAR.md` - Esta documentação

## ? Performance

- Renderização otimizada com `DoubleBuffered = true`
- Uso de `ResizeRedraw = true` para redimensionamento suave
- Atualização eficiente via `Invalidate()`
- Sem dependências externas além do System.Drawing
- GDI+ otimizado para desenho de segmentos

## ?? Notas Técnicas

- O controle herda de `System.Windows.Forms.UserControl`
- Usa `Graphics` e `GDI+` para renderização customizada
- Totalmente responsivo ao redimensionamento
- Compatible com .NET 10
- **100% Windows Forms nativo** - Sem necessidade de WPF ou ElementHost
- Suporta todas as propriedades padrão de controles WinForms (Dock, Anchor, etc.)
- Aparece automaticamente na Toolbox após compilar

## ?? Requisitos

- .NET 10 ou superior
- Windows Forms
- System.Drawing (incluído no .NET)
- Visual Studio 2022 ou superior

## ??? Localização do Controle

```
Namespace: Conector4.Controls
Classe: SegmentedProgressBar
Arquivo: Conector4\Controls\SegmentedProgressBar.cs
Toolbox: Seção "Conector4.Controls" (após compilar)
```

## ?? Dicas de Uso

1. **Use `Application.DoEvents()`** quando atualizar em loops para manter a UI responsiva
2. **Prefira `async/await`** para operações longas
3. **Defina `Maximum`** antes de começar o processamento
4. **Use `Dock` ou `Anchor`** para layout responsivo
5. **Combine com `BackgroundWorker`** para operações em background
6. **Compile o projeto** para ver o controle na Toolbox
7. **Use `BringToFront()`** se adicionar dinamicamente sobre outros controles

## ?? Customizações Possíveis

```csharp
// Alterar número de segmentos
progressBar.SegmentCount = 20; // Segmentos mais largos
progressBar.SegmentCount = 50; // Segmentos mais finos

// Alterar tamanho
progressBar.Height = 80; // Barra mais alta
progressBar.Width = 1000; // Barra mais larga

// Ocultar porcentagem
progressBar.ShowPercentage = false;

// Usar como indicador de nível
var nivelBarra = new SegmentedProgressBar
{
    ShowPercentage = false,
    SegmentCount = 10,
    Height = 30,
    Maximum = 10, // 10 níveis
    Value = 7 // Nível atual
};
```

## ?? Melhorias Futuras Possíveis

- [ ] Animação de transição entre valores
- [ ] Cores customizáveis via propriedades
- [ ] Orientação vertical
- [ ] Temas pré-definidos (Azul, Verde, Vermelho, Amarelo)
- [ ] Efeitos de brilho/pulsação
- [ ] Suporte a texto customizado (além da porcentagem)
- [ ] Modo indeterminado (animação contínua sem valor definido)
- [ ] Suporte a múltiplas barras (sub-progresso)
- [ ] Eventos customizados (OnValueChanged, OnCompleted)
