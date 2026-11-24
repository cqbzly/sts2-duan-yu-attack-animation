namespace DuanYuAttackAnimation;

internal static class AttackPoseArt
{
    private const string Ink = "#102D33";
    private const string Robe = "#215B65";
    private const string Sleeve = "#317E7E";
    private const string Jade = "#4EBDA8";
    private const string Silk = "#8AD2BB";
    private const string Gold = "#E6C47B";
    private const string Paper = "#F2ECD7";
    private const string Skin = "#E9C6A4";

    // Positions use the original combat sprite's 240 x 320 canvas. Feet stay on y=309.
    // At release, the palm heel is at (228,147) and the pointing fingertip is at (232,145).
    private static readonly Pose[] Poses =
    [
        new(
            HeadTransform: "translate(-13 10) rotate(-5 120 108)",
            Boots: "M73 270L66 291L54 301Q50 306 57 309H90L101 274Z M129 274L135 301Q136 308 145 309H177Q181 303 167 297L151 271Z",
            BackSleeve: "M89 146Q64 143 51 160L23 190Q33 202 56 201L87 180Z",
            BackHand: "M28 185L14 188L8 198Q8 204 15 202L30 198L39 193Z",
            Robe: "M76 137Q53 149 60 184L64 210L41 294Q66 304 98 290L117 246L139 297Q160 308 187 295L157 207Q163 181 147 157L128 133Z",
            Lining: "M89 142L113 176L127 141L139 164L127 211L146 284Q129 279 116 252L94 291L69 290L89 208Z",
            Collar: "M85 138L111 163L127 137L144 147L118 186L77 152Z",
            Folds: "M87 183L82 210L61 286 M108 221L92 268 M139 203Q148 246 167 289",
            Belt: "M66 199Q108 208 154 190L157 205Q113 226 65 213Z",
            Pendant: "M120 205L130 202L143 230L136 243L126 236Z",
            FrontSleeve: "M139 153Q157 153 170 169L177 184Q181 193 171 200L137 211L122 193L147 180L130 173",
            Cuff: "M128 191L138 211L125 216L114 197Z",
            FrontHand: "M116 197L106 190L92 196Q89 201 95 201L101 201L95 208Q94 212 99 214L107 210L112 215L125 213Z"),
        new(
            HeadTransform: "translate(17 7) rotate(9 120 108)",
            Boots: "M78 268L67 289L47 302Q42 307 49 309H84L104 283Z M152 270L170 299L188 301Q195 304 193 309H158L141 284Z",
            BackSleeve: "M108 134Q82 135 69 155L40 174L23 168Q16 176 22 192L52 199Q90 184 117 157Z",
            BackHand: "M27 170L20 163L8 164Q4 167 10 171L15 174L10 180Q9 185 16 185L25 187L32 177Z",
            Robe: "M103 139Q88 134 78 169L72 209Q69 238 43 278L31 295Q59 310 95 296L129 257L144 293Q162 305 185 299L163 225Q161 192 177 166L158 138Z",
            Lining: "M109 143L135 177L155 140L166 157L145 211L163 286L144 286L130 242L93 286L65 290L101 212Z",
            Collar: "M110 139L136 162L153 136L169 150L143 184L98 152Z",
            Folds: "M106 183Q91 237 59 287 M121 231L99 272 M153 207Q152 245 173 290",
            Belt: "M78 200Q119 208 161 197L163 210Q110 225 76 216Z",
            Pendant: "M113 208L125 207L121 234L110 245L103 234Z",
            FrontSleeve: "M158 147Q181 138 194 144L220 141L223 163L199 172L171 176Q158 173 151 163",
            Cuff: "M208 141L220 140L224 165L213 170Z",
            FrontHand: "M221 145L221 134L222 123Q224 119 227 123L228 133L231 119Q234 117 235 121L234 134L236 127Q238 126 238 130L236 142Q235 152 227 158L223 161Z"),
        new(
            HeadTransform: "translate(-6 0) rotate(-7 120 108)",
            Boots: "M78 271L75 293L65 301Q61 305 65 309H101L106 274Z M127 273L131 296Q130 304 138 309H167Q170 304 161 299L149 271Z",
            BackSleeve: "M83 144Q61 148 56 170L42 203L68 218L95 187L110 155Z",
            BackHand: "M49 204L62 210L78 199Q84 198 84 202L79 207L92 208Q97 210 91 214L76 222L61 226L47 215Z",
            Robe: "M82 132Q60 151 58 183L67 221L52 293Q75 302 102 294L122 264L142 297Q166 301 187 291L157 221L164 176Q157 147 139 130Z",
            Lining: "M92 137L115 173L134 135L151 157L138 210L153 288L141 288L121 251L99 285L76 285L91 212Z",
            Collar: "M87 132L114 163L136 131L149 146L121 185L79 149Z",
            Folds: "M92 179L83 217L68 285 M111 230L93 277 M146 205L165 282",
            Belt: "M67 202Q112 214 158 202L159 217Q112 228 67 216Z",
            Pendant: "M115 211L126 211L134 241L123 251L115 241Z",
            FrontSleeve: "M143 144Q162 148 170 170L188 189L171 207Q149 202 138 189L122 166",
            Cuff: "M169 187L184 173L194 185L177 203Z",
            FrontHand: "M185 180L176 168L167 154Q165 150 169 148Q172 148 175 152L179 158L175 146Q174 140 179 141L189 157Q199 157 199 167L194 180L189 185Z"),
        new(
            HeadTransform: "translate(10 3) rotate(5 120 108)",
            Boots: "M79 271L74 292L59 300Q54 306 61 309H93L103 278Z M140 269L151 295L166 300Q173 303 171 309H138L127 282Z",
            BackSleeve: "M97 139Q75 141 60 159L39 195L55 209L85 190L115 166Z",
            BackHand: "M44 192L30 188L17 192Q13 196 18 199L26 199L21 206Q20 211 25 211L39 207L48 202Z",
            Robe: "M95 135Q72 145 69 175L74 207L54 262L43 294Q66 305 96 292L122 256L135 294Q160 304 187 293L164 234Q156 206 165 178L153 134Z",
            Lining: "M101 138L130 175L151 137L164 156L145 207L157 286L139 285L122 238L94 280L68 289L98 213Z",
            Collar: "M98 136L130 162L149 134L164 148L138 183L89 150Z",
            Folds: "M98 184Q91 236 60 284 M116 219L90 277 M147 207Q151 238 170 283",
            Belt: "M75 201Q113 212 159 199L160 212Q113 225 74 216Z",
            Pendant: "M113 208L123 210L125 240L115 250L107 240Z",
            FrontSleeve: "M151 147Q170 143 186 153L208 147L214 166L190 177Q165 180 148 173L138 163",
            Cuff: "M199 149L208 145L215 166L205 170Z",
            FrontHand: "M209 148Q213 142 220 140L229 142Q234 143 232 147L221 147L231 150Q236 153 233 156L222 155Q226 159 220 164L213 165Z"),
        new(
            HeadTransform: "translate(8 3) rotate(3 120 108)",
            Boots: "M81 272L78 293L61 300Q56 304 61 309H96L107 275Z M139 273L143 294L160 301Q166 304 166 309H134L127 277Z",
            BackSleeve: "M93 139Q68 151 64 172L44 203L60 220L91 191L109 170Z",
            BackHand: "M48 201L38 200L28 207Q26 211 32 213L37 211L36 218Q38 222 43 219L52 214L55 207Z",
            Robe: "M91 134Q66 148 63 184L71 217Q61 259 45 293Q71 303 102 294L122 270L135 296Q161 305 182 294L160 221Q156 190 167 169Q157 144 145 132Z",
            Lining: "M98 141L123 177L145 136L157 155L141 212L156 287L138 286L124 255L98 288L71 289L95 213Z",
            Collar: "M95 136L124 163L143 134L158 146L131 184L86 151Z",
            Folds: "M93 183L85 217Q76 253 61 282 M107 227L96 267 M149 203Q154 248 170 287",
            Belt: "M72 202Q110 211 157 199L160 213Q113 226 72 217Z",
            Pendant: "M117 211L128 208L138 234L131 246L121 239Z",
            FrontSleeve: "M145 148Q165 149 175 168L196 176L190 195L173 194Q156 190 141 175",
            Cuff: "M187 172L199 175L194 197L182 194Z",
            FrontHand: "M199 176L209 168Q214 164 217 167L213 174L228 169Q232 170 230 174L218 179L224 180Q228 182 225 185L213 191Q207 195 195 193Z"),
    ];

    internal static string Render(int frame)
    {
        Pose pose = Poses[frame];
        return $$"""
            <svg xmlns="http://www.w3.org/2000/svg" width="240" height="320" viewBox="0 0 240 320">
              <ellipse cx="119" cy="305" rx="79" ry="10" fill="{{Ink}}" opacity=".22"/>
              <g stroke="{{Ink}}" stroke-linecap="round" stroke-linejoin="round">
                <path d="{{pose.Boots}}" fill="{{Ink}}" stroke-width="1.5"/>
                <path d="{{pose.BackHand}}" fill="{{Skin}}" stroke-width="2.2"/>
                <path d="{{pose.BackSleeve}}" fill="{{Robe}}" stroke-width="4"/>
                <path d="{{pose.Robe}}" fill="{{Robe}}" stroke-width="5"/>
                <path d="{{pose.Lining}}" fill="{{Jade}}" stroke="none"/>
                <path d="{{pose.Collar}}" fill="{{Paper}}" stroke="none"/>
                <path d="{{pose.Folds}}" fill="none" stroke="{{Silk}}" stroke-width="2.7" opacity=".65"/>
                <path d="{{pose.Belt}}" fill="{{Ink}}" stroke="none"/>
                <path d="{{pose.Pendant}}" fill="{{Gold}}" stroke-width="1.5"/>
                <g transform="{{pose.HeadTransform}}">
                  <path d="M87 70Q72 104 80 143M145 66Q161 104 153 145" fill="none" stroke-width="7"/>
                  <path d="M91 89Q86 53 119 51Q154 53 147 92L144 117Q130 138 115 132Q95 127 91 106Z" fill="{{Skin}}" stroke-width="4"/>
                  <path d="M89 92Q76 51 101 41Q112 22 139 40Q164 55 149 91L139 65Q108 77 93 68Z" fill="{{Ink}}" stroke="none"/>
                  <path d="M105 40Q96 17 119 12Q143 15 135 41Z" fill="{{Ink}}" stroke="none"/>
                  <path d="M103 33L138 32L138 42L104 44Z" fill="{{Gold}}" stroke="none"/>
                  <path d="M106 29L85 22M132 30L157 18" fill="none" stroke="{{Jade}}" stroke-width="4"/>
                  <path d="M101 91L111 89M130 89L140 91" fill="none" stroke-width="3"/>
                  <path d="M119 95L116 108L122 109M111 119Q122 123 131 117" fill="none" stroke="#AA7258" stroke-width="2"/>
                </g>
                <path d="{{pose.FrontSleeve}}" fill="{{Sleeve}}" stroke-width="4"/>
                <path d="{{pose.Cuff}}" fill="{{Jade}}" stroke-width="2"/>
                <path d="{{pose.FrontHand}}" fill="{{Skin}}" stroke-width="2"/>
              </g>
            </svg>
            """;
    }

    private sealed record Pose(
        string HeadTransform,
        string Boots,
        string BackSleeve,
        string BackHand,
        string Robe,
        string Lining,
        string Collar,
        string Folds,
        string Belt,
        string Pendant,
        string FrontSleeve,
        string Cuff,
        string FrontHand);
}
