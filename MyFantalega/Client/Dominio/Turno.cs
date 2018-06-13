using System;
using System.Text;
using fantacalcio.DominioLega;

namespace fantacalcio.DominioLega
{
    public class Turno
    {
        private Boolean _tipo;
        private Asta _astaAttiva;

        public Turno (Boolean tipo, Asta astaAttiva)
        {
            this._tipo = tipo;
            this._astaAttiva = astaAttiva;
        }

        public bool Tipo { get => _tipo; }
        public Asta AstaAttiva { get => _astaAttiva; }
    }
}