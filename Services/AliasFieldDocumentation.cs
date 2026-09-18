namespace BO3SoundSuite.Services;

/// <summary>
/// Human-readable help for Black Ops III sound-alias CSV fields.
/// Text is deliberately paraphrased from BO3 sound-alias documentation and long-running
/// UGX/Modme community references instead of guessing undocumented engine behavior.
/// </summary>
internal static class AliasFieldDocumentation
{
    private static readonly Dictionary<string, (string Fr, string En)> Documented =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Name"] = (
                "Nom de l'alias utilisé par le moteur, les scripts, animations et triggers. Plusieurs lignes portant exactement le même nom peuvent former un groupe de variantes choisi aléatoirement.",
                "Alias name used by the engine, scripts, animations and triggers. Multiple rows with exactly the same name can form a pool of variants selected at random."),
            ["Storage"] = (
                "Mode de stockage du son. 'loaded' charge le son en mémoire ; 'streamed' le lit en streaming. Les sons loaded vont dans les banques .sabl et les sons streamed dans les .sabs.",
                "Sound storage mode. 'loaded' keeps the sound in memory; 'streamed' reads it as a stream. Loaded sounds go to .sabl banks and streamed sounds to .sabs banks."),
            ["FileSpec"] = (
                "Chemin du WAV utilisé par l'alias, normalement relatif à sound_assets. Un dossier peut aussi être indiqué pour laisser le moteur choisir une variation parmi les WAV qu'il contient.",
                "Path of the WAV used by the alias, normally relative to sound_assets. A folder can also be specified so the engine can choose a variation from the WAV files it contains."),
            ["FileSpecSustain"] = (
                "Asset de sustain, généralement une boucle, lancé après la fin du one-shot défini dans FileSpec.",
                "Sustain asset, usually a loop, started after the one-shot in FileSpec finishes."),
            ["FileSpecRelease"] = (
                "Asset de release joué lorsque le sustain défini dans FileSpecSustain est arrêté.",
                "Release asset played when the sustain sound in FileSpecSustain is stopped."),
            ["Template"] = (
                "Nom d'un template d'alias défini dans les CSV de share\\raw\\sound\\templates. Le template fournit des valeurs de base réutilisables.",
                "Name of an alias template defined in CSV files under share\\raw\\sound\\templates. The template supplies reusable base values."),
            ["Loadspec"] = (
                "Nom d'un loadspec défini dans share\\raw\\sound\\globals\\loadspec.csv. Il sert à filtrer/grouper les alias inclus par une source SZC selon les Specs demandés.",
                "Name of a loadspec defined in share\\raw\\sound\\globals\\loadspec.csv. It is used to filter/group aliases included by an SZC source according to requested Specs."),
            ["Secondary"] = (
                "Nom d'un autre alias déclenché immédiatement après l'alias principal. Les secondaries peuvent être chaînés pour produire plusieurs couches sonores avec un seul trigger.",
                "Name of another alias triggered immediately after the primary alias. Secondaries can be chained to play multiple sound layers from one trigger."),
            ["Bus"] = (
                "Bus de mixage auquel appartient le son. Exemples documentés : BUS_FX, BUS_VOICE, BUS_UI, BUS_MUSIC, BUS_MOVIE. Les bus servent notamment au contrôle de groupes de sons.",
                "Mix bus the sound belongs to. Documented examples include BUS_FX, BUS_VOICE, BUS_UI, BUS_MUSIC and BUS_MOVIE. Buses are used to control groups of sounds."),
            ["VolumeGroup"] = (
                "Groupe de volume défini dans share\\raw\\sound\\globals\\volume_group.csv. Permet de regrouper des sons et d'appliquer un scaler d'atténuation au groupe.",
                "Volume group defined in share\\raw\\sound\\globals\\volume_group.csv. It groups similar sounds and allows an attenuation scaler to be applied to the group."),
            ["Duck"] = (
                "Nom d'un duck déclenché pendant la lecture de cet alias. Un duck réduit temporairement le volume de groupes de sons selon sa configuration.",
                "Name of a duck triggered while this alias plays. A duck temporarily lowers the volume of configured sound groups."),
            ["DuckGroup"] = (
                "Groupe de duck auquel appartient cet alias. Les groupes disponibles sont définis dans share\\raw\\sound\\globals\\duck_group.csv.",
                "Duck group this alias belongs to. Available groups are defined in share\\raw\\sound\\globals\\duck_group.csv."),
            ["ReverbSend"] = (
                "Quantité du signal envoyée vers la réverbération. La documentation BO3 décrit 0 comme aucun envoi et 100 comme un envoi total vers le chemin wet.",
                "Amount of signal sent to reverb. BO3 documentation describes 0 as no send and 100 as full send into the wet path."),
            ["CenterSend"] = (
                "Permet de remplacer/augmenter l'envoi vers le canal central par rapport à la valeur prévue par le pan de l'alias.",
                "Allows the center-channel send to override/increase the center value otherwise provided by the alias pan."),
            ["VolMin"] = (
                "Volume minimum de l'alias. Le moteur peut choisir une valeur entre VolMin et VolMax lorsqu'elles diffèrent.",
                "Minimum alias volume. The engine can choose a value between VolMin and VolMax when they differ."),
            ["VolMax"] = (
                "Volume maximum de l'alias. Le moteur peut choisir une valeur entre VolMin et VolMax lorsqu'elles diffèrent.",
                "Maximum alias volume. The engine can choose a value between VolMin and VolMax when they differ."),
            ["DistMin"] = (
                "Pour un son 3D, distance jusqu'à laquelle le son reste à son niveau maximal avant de commencer son atténuation.",
                "For a 3D sound, distance up to which the sound remains at full level before attenuation begins."),
            ["DistMaxDry"] = (
                "Distance maximale de la composante dry d'un son 3D. L'atténuation se fait entre DistMin et DistMaxDry jusqu'au silence.",
                "Maximum distance of the dry component of a 3D sound. It attenuates between DistMin and DistMaxDry until silent."),
            ["DistMaxWet"] = (
                "Distance maximale de la composante réverbérée/wet. Peut être supérieure à DistMaxDry pour laisser la réverbération porter plus loin que le son direct.",
                "Maximum distance of the reverberated/wet component. It can be greater than DistMaxDry so reverb remains audible farther than the direct sound."),
            ["DryMinCurve"] = (
                "Courbe d'atténuation dry utilisée dans la première zone de distance. Les courbes disponibles proviennent de la configuration audio BO3.",
                "Dry attenuation curve used in the first distance region. Available curves come from BO3 audio configuration."),
            ["DryMaxCurve"] = (
                "Courbe d'atténuation dry utilisée entre DistMin et DistMaxDry.",
                "Dry attenuation curve used between DistMin and DistMaxDry."),
            ["WetMinCurve"] = (
                "Équivalent de DryMinCurve pour la composante wet/réverbérée.",
                "Wet/reverb equivalent of DryMinCurve."),
            ["WetMaxCurve"] = (
                "Équivalent de DryMaxCurve pour la composante wet/réverbérée.",
                "Wet/reverb equivalent of DryMaxCurve."),
            ["LimitCount"] = (
                "Nombre d'instances simultanées de cet alias autorisées avant application de LimitType.",
                "Number of simultaneous instances of this alias allowed before LimitType is applied."),
            ["LimitType"] = (
                "Action lorsque LimitCount est atteint : none = pas de limite, oldest = coupe la plus ancienne, reject = refuse la nouvelle instance, priority = choisit selon la priorité.",
                "Action when LimitCount is reached: none = no limit, oldest = stop the oldest, reject = reject the new instance, priority = choose according to priority."),
            ["EntityLimitCount"] = (
                "Comme LimitCount, mais le comptage est effectué séparément pour chaque entité qui joue l'alias.",
                "Like LimitCount, but the count is evaluated separately for each entity playing the alias."),
            ["EntityLimitType"] = (
                "Comme LimitType, mais appliqué à la limite d'instances de l'alias sur une entité particulière.",
                "Like LimitType, but applied to the per-entity instance limit for this alias."),
            ["PitchMin"] = (
                "Borne minimale de variation de pitch. Le moteur peut randomiser le pitch entre PitchMin et PitchMax selon le template/configuration utilisé.",
                "Minimum pitch-variation bound. The engine can randomize pitch between PitchMin and PitchMax depending on the template/configuration used."),
            ["PitchMax"] = (
                "Borne maximale de variation de pitch. Le moteur peut randomiser le pitch entre PitchMin et PitchMax selon le template/configuration utilisé.",
                "Maximum pitch-variation bound. The engine can randomize pitch between PitchMin and PitchMax depending on the template/configuration used."),
            ["PanType"] = (
                "Mode spatial : 2d = volume/pan indépendants de la position, 3d = volume/pan selon la position dans le monde, 2.5 = pan spatial sans atténuation par distance.",
                "Spatial mode: 2d = volume/pan independent of world position, 3d = volume/pan depend on world position, 2.5 = spatial panning without distance attenuation."),
            ["Pan"] = (
                "Nom d'un profil de pan défini dans share\\raw\\sound\\globals\\pan.csv. Il contrôle la répartition du son dans le champ de haut-parleurs.",
                "Name of a pan profile defined in share\\raw\\sound\\globals\\pan.csv. It controls how the sound is distributed across the speaker field."),
            ["Futz"] = (
                "Nom d'un effet/futz appliqué au son en temps réel. Les configurations correspondantes se trouvent dans share\\raw\\sound\\globals\\futz.csv.",
                "Name of a real-time futz/effect applied to the sound. Corresponding configurations are in share\\raw\\sound\\globals\\futz.csv."),
            ["Looping"] = (
                "NONLOOPING joue le son une fois. LOOPING le relance en boucle jusqu'à ce qu'il soit explicitement arrêté.",
                "NONLOOPING plays once. LOOPING continuously requeues the sound until it is explicitly stopped."),
            ["RandomizeType"] = (
                "Contrôle une randomisation stable par entité. Valeurs documentées : volume, pitch, variant. Vide = nouvelle randomisation à chaque instance.",
                "Controls stable randomization per entity. Documented values: volume, pitch, variant. Blank = new randomization for each sound instance."),
            ["Probability"] = (
                "Probabilité que le son soit réellement joué lorsqu'il est déclenché. 1 = 100 %, 0 = 0 %.",
                "Probability that the sound actually plays when triggered. 1 = 100%, 0 = 0%."),
            ["StartDelay"] = (
                "Délai en millisecondes entre le déclenchement de l'alias et le début de la lecture.",
                "Delay in milliseconds between triggering the alias and starting playback."),
            ["EnvelopMin"] = (
                "Rayon interne de l'effet d'enveloppement d'un son 3D. À proximité, une partie du signal peut être envoyée vers des canaux qui ne recevraient normalement pas ce son.",
                "Inner radius of the 3D envelop effect. At close range, part of the signal can be sent to channels that normally would not receive that sound."),
            ["EnvelopMax"] = (
                "Rayon externe de l'effet d'enveloppement. Entre EnvelopMin et EnvelopMax, l'effet diminue progressivement jusqu'à zéro.",
                "Outer radius of the envelop effect. Between EnvelopMin and EnvelopMax the effect scales down progressively to zero."),
            ["EnvelopPercent"] = (
                "Quantité de signal injectée dans les canaux supplémentaires par l'effet d'enveloppement.",
                "Amount of signal injected into additional channels by the envelop effect."),
            ["OcclusionLevel"] = (
                "Facteur 0 à 1 qui multiplie l'occlusion calculée en temps réel. 1 applique toute l'occlusion ; 0.5 en applique environ la moitié.",
                "0-to-1 factor scaling real-time calculated occlusion. 1 applies the full result; 0.5 applies roughly half."),
            ["IsBig"] = (
                "Pour un son 3D marqué comme 'big', l'occlusion basée sur la géométrie est désactivée lorsque l'auditeur est proche.",
                "For a 3D sound marked as 'big', geometry-based occlusion is disabled when the listener is close."),
            ["DistanceLpf"] = (
                "Active le filtrage passe-bas dépendant de la distance. Le moteur combine ce résultat avec le filtrage lié à l'occlusion.",
                "Enables distance-based low-pass filtering. The engine combines this result with occlusion-based filtering."),
            ["FluxType"] = (
                "Type de mouvement spatial artificiel appliqué au son sans l'attacher à un objet 3D mobile. Exemples documentés : left/right/center/random_player et left/center/right_shot.",
                "Type of artificial spatial movement applied without attaching the sound to a moving 3D object. Documented examples include left/right/center/random_player and left/center/right_shot."),
            ["FluxTime"] = (
                "Durée en millisecondes pendant laquelle l'effet Flux déplace la position sonore.",
                "Duration in milliseconds for which the Flux effect moves the sound position."),
            ["Subtitle"] = (
                "Sous-titre à afficher avec le son, principalement utilisé pour les lignes de dialogue.",
                "Subtitle to display with the sound, mainly used for dialogue lines."),
            ["Doppler"] = (
                "Active ou désactive l'effet Doppler sur cet alias.",
                "Enables or disables the Doppler effect for this alias."),
            ["ContextType"] = (
                "La documentation BO3 indique de ne pas utiliser les champs ContextType/ContextValue pour les alias utilisateur standards.",
                "BO3 documentation says not to use ContextType/ContextValue fields for normal user aliases."),
            ["ContextValue"] = (
                "La documentation BO3 indique de ne pas utiliser les champs ContextType/ContextValue pour les alias utilisateur standards.",
                "BO3 documentation says not to use ContextType/ContextValue fields for normal user aliases."),
            ["ContextType1"] = (
                "Variante supplémentaire des champs de contexte. Aucun usage utilisateur fiable n'est documenté ; conserver vide sauf reprise d'un alias Treyarch connu.",
                "Additional context field. No reliable user-authoring use is documented; leave blank unless reproducing a known Treyarch alias."),
            ["ContextValue1"] = (
                "Valeur associée à ContextType1. Aucun usage utilisateur fiable n'est documenté ; conserver vide sauf reprise d'un alias Treyarch connu.",
                "Value associated with ContextType1. No reliable user-authoring use is documented; leave blank unless reproducing a known Treyarch alias."),
            ["ContextType2"] = (
                "Variante supplémentaire des champs de contexte. Aucun usage utilisateur fiable n'est documenté ; conserver vide sauf reprise d'un alias Treyarch connu.",
                "Additional context field. No reliable user-authoring use is documented; leave blank unless reproducing a known Treyarch alias."),
            ["ContextValue2"] = (
                "Valeur associée à ContextType2. Aucun usage utilisateur fiable n'est documenté ; conserver vide sauf reprise d'un alias Treyarch connu.",
                "Value associated with ContextType2. No reliable user-authoring use is documented; leave blank unless reproducing a known Treyarch alias."),
            ["ContextType3"] = (
                "Variante supplémentaire des champs de contexte. Aucun usage utilisateur fiable n'est documenté ; conserver vide sauf reprise d'un alias Treyarch connu.",
                "Additional context field. No reliable user-authoring use is documented; leave blank unless reproducing a known Treyarch alias."),
            ["ContextValue3"] = (
                "Valeur associée à ContextType3. Aucun usage utilisateur fiable n'est documenté ; conserver vide sauf reprise d'un alias Treyarch connu.",
                "Value associated with ContextType3. No reliable user-authoring use is documented; leave blank unless reproducing a known Treyarch alias."),
            ["Timescale"] = (
                "Détermine si la lecture du son doit suivre le timescale global du jeu.",
                "Controls whether sound playback should be affected by the game's global timescale."),
            ["IsMusic"] = (
                "Indique si l'alias fait partie du système de musique.",
                "Marks whether the alias is part of the music system."),
            ["IsCinematic"] = (
                "Indique si l'alias appartient à une séquence cinématique.",
                "Marks whether the alias belongs to a cinematic sequence."),
            ["FadeIn"] = (
                "Durée en millisecondes du fondu d'entrée, du silence jusqu'au volume normal.",
                "Fade-in duration in milliseconds, from silence to normal playback volume."),
            ["FadeOut"] = (
                "Durée en millisecondes du fondu de sortie jusqu'au silence lorsque le son se termine/s'arrête.",
                "Fade-out duration in milliseconds to silence when the sound ends/stops."),
            ["Pauseable"] = (
                "YES permet au son de se mettre en pause lorsque le jeu est en pause. La documentation recommande YES pour la majorité des sons.",
                "YES allows the sound to pause when the game pauses. Documentation recommends YES for most sounds."),
            ["StopOnEntDeath"] = (
                "Détermine si le son répond à une demande d'arrêt des sons attachés à une entité, par exemple lors de sa mort.",
                "Controls whether the sound responds to a request to stop sounds attached to an entity, for example when that entity dies."),
            ["Compression"] = (
                "Qualité de compression 0-100 documentée pour consoles. Sur PC, la documentation indique que l'encodage utilise un ratio FLAC constant.",
                "0-100 compression-quality control documented for consoles. On PC, documentation states that encoding uses a constant FLAC compression ratio."),
            ["StopOnPlay"] = (
                "Champ indiqué comme non implémenté dans la documentation BO3 consultée.",
                "Field described as not implemented in the BO3 documentation consulted."),
            ["DopplerScale"] = (
                "Facteur qui multiplie l'intensité de l'effet Doppler appliqué à l'alias.",
                "Scale factor multiplying the Doppler effect applied to the alias."),
            ["FutzPatch"] = (
                "Nom d'un futz patch défini dans share\\raw\\sound\\globals\\futz.csv à appliquer au son.",
                "Name of a futz patch defined in share\\raw\\sound\\globals\\futz.csv to apply to the sound."),
            ["VoiceLimit"] = (
                "Champ indiqué comme inutilisé dans la documentation BO3 consultée.",
                "Field described as unused in the BO3 documentation consulted."),
            ["IgnoreMaxDist"] = (
                "YES empêche le moteur de rejeter immédiatement un événement 3D déclenché au-delà de DistMaxWet. Utile pour un son qui commence loin puis se rapproche.",
                "YES prevents the engine from immediately culling a 3D play event triggered beyond DistMaxWet. Useful for a sound that starts far away and approaches."),
            ["NeverPlayTwice"] = (
                "YES empêche l'alias de se déclencher plus d'une fois pendant le niveau/checkpoint courant. Le flag est réinitialisé lors d'un nouveau niveau ou d'un restart de checkpoint.",
                "YES prevents the alias from triggering more than once during the current level/checkpoint. The flag resets on a new level or checkpoint restart."),
            ["ContinuousPan"] = (
                "Contrôle le comportement des sons 3D non bouclés lors d'un déplacement instantané important de l'auditeur. La documentation indique que YES permet de stopper ces sons pour éviter des pops de distance.",
                "Controls non-looping 3D sounds when the listener instantly moves a large distance. Documentation says YES allows those sounds to be stopped to prevent distance-related pops."),
        };

    private static readonly Dictionary<string, (string Fr, string En)> CommunityDocumented =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["PriorityMin"] = (
                "Borne basse de priorité utilisée par les alias configurés avec un mode de limitation 'priority'. Les templates BO3 communautaires utilisent une plage Min/Max pour départager les instances.",
                "Lower priority bound used by aliases configured with the 'priority' limiting mode. Community BO3 templates use a Min/Max range to rank competing instances."),
            ["PriorityMax"] = (
                "Borne haute de priorité utilisée par les alias configurés avec un mode de limitation 'priority'. Avec LimitType/EntityLimitType = PRIORITY, snd_convert exige une plage valide et l'app interdit Min = Max.",
                "Upper priority bound used by aliases configured with the 'priority' limiting mode. With LimitType/EntityLimitType = PRIORITY, snd_convert requires a valid range and the app rejects Min = Max."),
            ["PriorityThresholdMin"] = (
                "Seuil bas associé au calcul de priorité. Des templates BO3 publiés sur les forums utilisent des valeurs normalisées dans cette colonne, mais la signification détaillée n'est pas expliquée dans la documentation publique consultée.",
                "Lower threshold associated with priority calculation. BO3 templates shared on forums use normalized values in this column, but its detailed semantics are not explained in the public documentation consulted."),
            ["PriorityThresholdMax"] = (
                "Seuil haut associé au calcul de priorité. Des templates BO3 publiés sur les forums utilisent des valeurs normalisées dans cette colonne, mais la signification détaillée n'est pas expliquée dans la documentation publique consultée.",
                "Upper threshold associated with priority calculation. BO3 templates shared on forums use normalized values in this column, but its detailed semantics are not explained in the public documentation consulted."),
        };

    public static string Get(string? field)
    {
        var name = (field ?? string.Empty).Trim();
        if (name.Length == 0) return string.Empty;

        if (Documented.TryGetValue(name, out var doc))
        {
            return $"{name}\n\n{Localization.T(doc.Fr, doc.En)}\n\n" +
                   Localization.T("Source : documentation BO3 « Sound Aliases ».", "Source: BO3 'Sound Aliases' documentation.");
        }

        if (CommunityDocumented.TryGetValue(name, out var community))
        {
            return $"{name}\n\n{Localization.T(community.Fr, community.En)}\n\n" +
                   Localization.T("Source : templates/alias BO3 publiés sur les forums Modme/UGX et comportement de snd_convert.", "Source: BO3 templates/aliases published on Modme/UGX forums and snd_convert behavior.");
        }

        return $"{name}\n\n" + Localization.T(
            "Cette colonne existe bien dans le format Sound Alias de Black Ops III, mais aucune description fiable de son rôle n'a été trouvée dans la documentation Mod Tools ou les références communautaires consultées. Laisse-la vide sauf si tu reproduis les valeurs d'un alias Treyarch connu.",
            "This column exists in the Black Ops III Sound Alias format, but no reliable description of its role was found in the Mod Tools documentation or community references consulted. Leave it blank unless reproducing values from a known Treyarch alias.") +
            "\n\n" + Localization.T(
                "Source : en-têtes BO3 Sound_Alias_Example/user_aliases et archives UGX/Modme.",
                "Source: BO3 Sound_Alias_Example/user_aliases headers and UGX/Modme archives.");
    }
}
