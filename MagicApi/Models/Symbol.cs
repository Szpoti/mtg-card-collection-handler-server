using System.Collections.Generic;
using Services.Models;

public class Symbol
{
    public Symbol()
    {
    }

    public Symbol(SymbolModel symbolModel)
    {
        Sym = symbolModel.symbol;
        Uri = symbolModel.svg_uri;
        Text = symbolModel.english;
    }

    public string Sym { get; set; }
    public string Uri { get; set; }
    public string Text { get; set; }

}