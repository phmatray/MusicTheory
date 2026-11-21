using MusicTheory.Toolbox.I18n;

namespace MusicTheory.Toolbox.Tools.Transposer;

public sealed class TransposerStrings : IStringTable
{
    public IReadOnlyDictionary<string, string> En { get; } = new Dictionary<string, string>
    {
        ["transpose.title"] = "Transposer",
        ["transpose.blurb"] = "Convert a key and a chord progression between concert pitch and the written part of a transposing instrument.",
        ["transpose.inst"] = "Instrument",
        ["transpose.inst.c"] = "{0} instruments, concert pitch (flute, piano, violin)",
        ["transpose.inst.bb"] = "{0} instruments (trumpet, clarinet, soprano sax)",
        ["transpose.inst.eb"] = "{0} instruments (alto sax, baritone sax)",
        ["transpose.inst.f"] = "{0} instruments (horn)",
        ["transpose.inst.bbt"] = "{0} tenor sax (a ninth above concert)",
        ["transpose.dir"] = "Direction",
        ["transpose.dir.cw"] = "Concert to written",
        ["transpose.dir.wc"] = "Written to concert",
        ["transpose.key"] = "Key (before)",
        ["transpose.chords"] = "Chords, separated by spaces",
        ["transpose.run"] = "Transpose",
        ["transpose.result"] = "Result",
        ["transpose.target"] = "Key (after)",
        ["transpose.signature"] = "Key signature",
        ["transpose.sig.none"] = "no sharps or flats",
        ["transpose.sig.sharps"] = "{0} sharp(s)",
        ["transpose.sig.flats"] = "{0} flat(s)",
        ["transpose.major"] = "{0} major",
        ["transpose.minor"] = "{0} minor",
        ["transpose.invalid"] = "Unknown key \"{0}\": using C major.",
        ["transpose.theoretical"] = "{0} needs more than seven accidentals, so {1} is used instead.",
        ["transpose.unparsed"] = "\"{0}\" is not a chord the library can read: kept as typed.",
        ["transpose.understand"] = "Concert pitch is the sound you hear. A transposing instrument reads a different note for the same sound: "
            + "when a B flat trumpet reads C, you hear B flat. To write a part for it from concert pitch, move everything up a major second; "
            + "for E flat alto sax a major sixth, for horn in F a fifth. To go back, move down by the same interval. "
            + "The key is moved with the fewest accidentals and each chord is respelled in the new key.",
    };

    public IReadOnlyDictionary<string, string> Fr { get; } = new Dictionary<string, string>
    {
        ["transpose.title"] = "Transpositeur",
        ["transpose.blurb"] = "Convertissez une tonalité et une grille d'accords entre le son réel et la partie écrite d'un instrument transpositeur.",
        ["transpose.inst"] = "Instrument",
        ["transpose.inst.c"] = "Instruments en {0}, son réel (flûte, piano, violon)",
        ["transpose.inst.bb"] = "Instruments en {0} (trompette, clarinette, sax soprano)",
        ["transpose.inst.eb"] = "Instruments en {0} (sax alto, sax baryton)",
        ["transpose.inst.f"] = "Instruments en {0} (cor)",
        ["transpose.inst.bbt"] = "Sax ténor en {0} (une neuvième au-dessus du son réel)",
        ["transpose.dir"] = "Sens",
        ["transpose.dir.cw"] = "Son réel vers partie écrite",
        ["transpose.dir.wc"] = "Partie écrite vers son réel",
        ["transpose.key"] = "Tonalité (avant)",
        ["transpose.chords"] = "Accords, séparés par des espaces",
        ["transpose.run"] = "Transposer",
        ["transpose.result"] = "Résultat",
        ["transpose.target"] = "Tonalité (après)",
        ["transpose.signature"] = "Armure",
        ["transpose.sig.none"] = "aucune altération",
        ["transpose.sig.sharps"] = "{0} dièse(s)",
        ["transpose.sig.flats"] = "{0} bémol(s)",
        ["transpose.major"] = "{0} majeur",
        ["transpose.minor"] = "{0} mineur",
        ["transpose.invalid"] = "Tonalité inconnue \"{0}\" : utilisation de Do majeur.",
        ["transpose.theoretical"] = "{0} demande plus de sept altérations : {1} est utilisé à la place.",
        ["transpose.unparsed"] = "\"{0}\" n'est pas un accord lisible par la bibliothèque : conservé tel quel.",
        ["transpose.understand"] = "Le son réel est ce que l'on entend. Un instrument transpositeur lit une autre note pour le même son : "
            + "quand une trompette en si bémol lit un do, on entend un si bémol. Pour écrire sa partie depuis le son réel, on monte tout d'une seconde majeure ; "
            + "d'une sixte majeure pour le sax alto en mi bémol, d'une quinte pour le cor en fa. Pour revenir, on descend du même intervalle. "
            + "La tonalité est déplacée avec le moins d'altérations possible et chaque accord est réécrit dans la nouvelle tonalité.",
    };
}
