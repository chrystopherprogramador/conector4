# Melhorias para a Aba Serviços

## Problemas Identificados:

1. **Falta o campo `time`** no INSERT/UPDATE
2. **Conversão incorreta do campo `ativo`** - boolean vs SMALLINT
3. **Falta validação de campos obrigatórios**
4. **Falta campo `txtTime` na interface**
5. **Falta tratamento do campo `time` ao editar**

## Alterações Necessárias:

### 1. Adicionar campo txtTime no Designer (MDI.Designer.cs)
```csharp
// Adicione este controle ao formulário na aba Serviços
private System.Windows.Forms.TextBox txtTime;
```

### 2. Substituir o método `btnGravar_Click` no MDI.cs:

```csharp
private async void btnGravar_Click(object sender, EventArgs e)
{
    try
    {
        // Validações
        if (string.IsNullOrWhiteSpace(txtNomeServico.Text))
        {
            MessageBox.Show("O campo Nome é obrigatório!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtNomeServico.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtClasse.Text))
        {
            MessageBox.Show("O campo Classe é obrigatório!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtClasse.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtMetodo.Text))
        {
            MessageBox.Show("O campo Método é obrigatório!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtMetodo.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtTime.Text) || !int.TryParse(txtTime.Text, out int time) || time < 0)
        {
            MessageBox.Show("O campo Time deve ser um número inteiro maior ou igual a zero!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTime.Focus();
            return;
        }

        // Captura os valores dos campos
        int codigo = string.IsNullOrWhiteSpace(txtCodigo.Text) ? 0 : int.Parse(txtCodigo.Text);
        string nome = txtNomeServico.Text.Trim();
        short ativo = chAtivoServico.Checked ? (short)1 : (short)0;
        string classe = txtClasse.Text.Trim();
        string metodo = txtMetodo.Text.Trim();

        var _config = new clsConfig();

        await using var conn = new NpgsqlConnection(_config.getConfigBD());
        await conn.OpenAsync();

        string sql;

        if (codigo == 0) // INSERT
        {
            sql = @"INSERT INTO servicos (nome, ativo, time, classe, metodo)
                    VALUES (@nome, @ativo, @time, @classe, @metodo);";
        }
        else // UPDATE
        {
            sql = @"UPDATE servicos
                    SET nome = @nome, ativo = @ativo, time = @time, classe = @classe, metodo = @metodo
                    WHERE id = @id;";
        }

        await using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("nome", nome);
        cmd.Parameters.AddWithValue("ativo", ativo);
        cmd.Parameters.AddWithValue("time", time);
        cmd.Parameters.AddWithValue("classe", classe);
        cmd.Parameters.AddWithValue("metodo", metodo);

        if (codigo != 0)
            cmd.Parameters.AddWithValue("id", codigo);

        await cmd.ExecuteNonQueryAsync();

        MessageBox.Show(codigo == 0 ? "Registro inserido com sucesso!" : "Registro atualizado com sucesso!", 
                        "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

        LimparCamposServico();
        var ds = await fn.returnServico(); 
        GridServico.DataSource = ds.Tables[0];
    }
    catch (Exception ex)
    {
        MessageBox.Show("Erro ao gravar registro: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

### 3. Adicionar novo método `LimparCamposServico`:

```csharp
private void LimparCamposServico()
{
    txtCodigo.Text = "";
    txtNomeServico.Text = "";
    txtTime.Text = "60";
    chAtivoServico.Checked = true;
    txtClasse.Text = "";
    txtMetodo.Text = "";
    txtNomeServico.Focus();
}
```

### 4. Substituir o método `GridServico_CellDoubleClick`:

```csharp
private void GridServico_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
{
    if (e.RowIndex >= 0)
    {
        DataGridViewRow row = GridServico.Rows[e.RowIndex];

        txtCodigo.Text = row.Cells["Codigo"].Value?.ToString();
        txtNomeServico.Text = row.Cells["Serviço"].Value?.ToString();
        
        // Converte ativo corretamente (1 ou 0 para boolean)
        if (row.Cells["Ativo"].Value != null)
        {
            var ativoValue = row.Cells["Ativo"].Value.ToString();
            chAtivoServico.Checked = ativoValue == "1" || ativoValue.ToLower() == "true";
        }
        else
        {
            chAtivoServico.Checked = false;
        }

        txtClasse.Text = row.Cells["Classe"].Value?.ToString();
        txtMetodo.Text = row.Cells["Metodo"].Value?.ToString();
        
        // Busca o time do banco
        _ = CarregarTimeServico(int.Parse(txtCodigo.Text));
    }
}
```

### 5. Adicionar método `CarregarTimeServico`:

```csharp
private async Task CarregarTimeServico(int id)
{
    try
    {
        var _config = new clsConfig();
        await using var conn = new NpgsqlConnection(_config.getConfigBD());
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand("SELECT time FROM servicos WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);

        var result = await cmd.ExecuteScalarAsync();
        if (result != null)
        {
            txtTime.Text = result.ToString();
        }
    }
    catch (Exception ex)
    {
        await _logger.LogAsync("Error", $"Erro ao carregar time do serviço: {ex.Message}");
    }
}
```

### 6. Adicionar botões Novo e Excluir (opcional mas recomendado):

```csharp
private void btnNovoServico_Click(object sender, EventArgs e)
{
    LimparCamposServico();
}

private async void btnExcluirServico_Click(object sender, EventArgs e)
{
    try
    {
        if (string.IsNullOrWhiteSpace(txtCodigo.Text))
        {
            MessageBox.Show("Selecione um registro para excluir!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var result = MessageBox.Show("Tem certeza que deseja excluir este serviço?", 
                                    "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            int codigo = int.Parse(txtCodigo.Text);
            var _config = new clsConfig();

            await using var conn = new NpgsqlConnection(_config.getConfigBD());
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand("DELETE FROM servicos WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", codigo);

            await cmd.ExecuteNonQueryAsync();

            MessageBox.Show("Registro excluído com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LimparCamposServico();
            var ds = await fn.returnServico();
            GridServico.DataSource = ds.Tables[0];
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show("Erro ao excluir registro: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

### 7. Atualizar o método MDI_Load para adicionar a fila de Produtos:

```csharp
// No método MDI_Load, adicione esta linha:
cmbServicoMensageria.Items.Add("Presence.Produtos");
```

## Resumo das Melhorias:

? Adiciona validações de campos obrigatórios  
? Corrige conversão do campo `ativo` (boolean ? SMALLINT)  
? Adiciona suporte ao campo `time` no INSERT/UPDATE  
? Adiciona método para limpar campos  
? Corrige carregamento do campo `ativo` ao editar  
? Adiciona carregamento do campo `time` ao editar  
? Adiciona botão para novo registro  
? Adiciona botão para excluir registro  
? Melhora mensagens de feedback ao usuário  
? Adiciona fila "Presence.Produtos" no combobox
