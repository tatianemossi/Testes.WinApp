using Testes.WinApp.Compartilhado;

namespace Testes.WinApp.ModutoTeste
{
    public class ConfiguracaoToolboxTeste : ConfiguracaoToolboxBase
    {
        public override string TipoCadastro => "Cadastro de Testes";

        public override string TooltipInserir { get { return "Inserir um novo Teste"; } }

        public override string TooltipEditar { get { return "Editar um Teste existente"; } }

        public override string TooltipExcluir { get { return "Excluir um Teste existente"; } }
    }
}
