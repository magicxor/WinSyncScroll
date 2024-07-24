// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming

namespace Windows.Win32;

public static class WinApiConstants
{
    /// HELP_WM_HELP -> 0x000c
    public const int HELP_WM_HELP = 12;

    /// WM_INPUTLANGCHANGEREQUEST -> 0x0050
    public const int WM_INPUTLANGCHANGEREQUEST = 80;

    /// WM_CHOOSEFONT_SETLOGFONT -> (WM_USER + 101)
    public const int WM_CHOOSEFONT_SETLOGFONT = (WinApiConstants.WM_USER + 101);

    /// WM_CHOOSEFONT_GETLOGFONT -> (WM_USER + 1)
    public const int WM_CHOOSEFONT_GETLOGFONT = (WinApiConstants.WM_USER + 1);

    /// WM_IME_STARTCOMPOSITION -> 0x010D
    public const int WM_IME_STARTCOMPOSITION = 269;

    /// WM_IME_COMPOSITIONFULL -> 0x0284
    public const int WM_IME_COMPOSITIONFULL = 644;

    /// WM_CHOOSEFONT_SETFLAGS -> (WM_USER + 102)
    public const int WM_CHOOSEFONT_SETFLAGS = (WinApiConstants.WM_USER + 102);

    /// WM_PSD_YAFULLPAGERECT -> (WM_USER+6)
    public const int WM_PSD_YAFULLPAGERECT = (WinApiConstants.WM_USER + 6);

    /// WM_IME_ENDCOMPOSITION -> 0x010E
    public const int WM_IME_ENDCOMPOSITION = 270;

    /// WM_WTSSESSION_CHANGE -> 0x02B1
    public const int WM_WTSSESSION_CHANGE = 689;

    /// WM_WINDOWPOSCHANGING -> 0x0046
    public const int WM_WINDOWPOSCHANGING = 70;

    /// WM_PSD_MINMARGINRECT -> (WM_USER+2)
    public const int WM_PSD_MINMARGINRECT = (WinApiConstants.WM_USER + 2);

    /// WM_PSD_GREEKTEXTRECT -> (WM_USER+4)
    public const int WM_PSD_GREEKTEXTRECT = (WinApiConstants.WM_USER + 4);

    /// WM_PALETTEISCHANGING -> 0x0310
    public const int WM_PALETTEISCHANGING = 784;

    /// WM_CTLCOLORSCROLLBAR -> 0x0137
    public const int WM_CTLCOLORSCROLLBAR = 311;

    /// WM_WINDOWPOSCHANGED -> 0x0047
    public const int WM_WINDOWPOSCHANGED = 71;

    /// WM_VSCROLLCLIPBOARD -> 0x030A
    public const int WM_VSCROLLCLIPBOARD = 778;

    /// WM_RENDERALLFORMATS -> 0x0306
    public const int WM_RENDERALLFORMATS = 774;

    /// WM_PSD_PAGESETUPDLG -> (WM_USER  )
    public const int WM_PSD_PAGESETUPDLG = WinApiConstants.WM_USER;

    /// WM_PSD_FULLPAGERECT -> (WM_USER+1)
    public const int WM_PSD_FULLPAGERECT = (WinApiConstants.WM_USER + 1);

    /// WM_PSD_ENVSTAMPRECT -> (WM_USER+5)
    public const int WM_PSD_ENVSTAMPRECT = (WinApiConstants.WM_USER + 5);

    /// WM_HSCROLLCLIPBOARD -> 0x030E
    public const int WM_HSCROLLCLIPBOARD = 782;

    /// WM_DESTROYCLIPBOARD -> 0x0307
    public const int WM_DESTROYCLIPBOARD = 775;

    /// WM_UNINITMENUPOPUP -> 0x0125
    public const int WM_UNINITMENUPOPUP = 293;

    /// WM_QUERYNEWPALETTE -> 0x030F
    public const int WM_QUERYNEWPALETTE = 783;

    /// WM_QUERYENDSESSION -> 0x0011
    public const int WM_QUERYENDSESSION = 17;

    /// WM_NCXBUTTONDBLCLK -> 0x00AD
    public const int WM_NCXBUTTONDBLCLK = 173;

    /// WM_NCRBUTTONDBLCLK -> 0x00A6
    public const int WM_NCRBUTTONDBLCLK = 166;

    /// WM_NCMBUTTONDBLCLK -> 0x00A9
    public const int WM_NCMBUTTONDBLCLK = 169;

    /// WM_NCLBUTTONDBLCLK -> 0x00A3
    public const int WM_NCLBUTTONDBLCLK = 163;

    /// WM_INPUTLANGCHANGE -> 0x0051
    public const int WM_INPUTLANGCHANGE = 81;

    /// WM_IME_COMPOSITION -> 0x010F
    public const int WM_IME_COMPOSITION = 271;

    /// WM_CTLCOLORLISTBOX -> 0x0134
    public const int WM_CTLCOLORLISTBOX = 308;

    /// WM_ASKCBFORMATNAME -> 0x030C
    public const int WM_ASKCBFORMATNAME = 780;

    /// WM_SYSCOLORCHANGE -> 0x0015
    public const int WM_SYSCOLORCHANGE = 21;

    /// WM_PSD_MARGINRECT -> (WM_USER+3)
    public const int WM_PSD_MARGINRECT = (WinApiConstants.WM_USER + 3);

    /// WM_POWERBROADCAST -> 0x0218
    public const int WM_POWERBROADCAST = 536;

    /// WM_PALETTECHANGED -> 0x0311
    public const int WM_PALETTECHANGED = 785;

    /// WM_PAINTCLIPBOARD -> 0x0309
    public const int WM_PAINTCLIPBOARD = 777;

    /// WM_MDIREFRESHMENU -> 0x0234
    public const int WM_MDIREFRESHMENU = 564;

    /// WM_MDIICONARRANGE -> 0x0228
    public const int WM_MDIICONARRANGE = 552;

    /// WM_IME_SETCONTEXT -> 0x0281
    public const int WM_IME_SETCONTEXT = 641;

    /// WM_ICONERASEBKGND -> 0x0027
    public const int WM_ICONERASEBKGND = 39;

    /// WM_CTLCOLORSTATIC -> 0x0138
    public const int WM_CTLCOLORSTATIC = 312;

    /// WM_CTLCOLORMSGBOX -> 0x0132
    public const int WM_CTLCOLORMSGBOX = 306;

    /// WM_CAPTURECHANGED -> 0x0215
    public const int WM_CAPTURECHANGED = 533;

    /// WM_XBUTTONDBLCLK -> 0x020D
    public const int WM_XBUTTONDBLCLK = 525;

    /// WM_UPDATEUISTATE -> 0x0128
    public const int WM_UPDATEUISTATE = 296;

    /// WM_STYLECHANGING -> 0x007C
    public const int WM_STYLECHANGING = 124;

    /// WM_SPOOLERSTATUS -> 0x002A
    public const int WM_SPOOLERSTATUS = 42;

    /// WM_SIZECLIPBOARD -> 0x030B
    public const int WM_SIZECLIPBOARD = 779;

    /// WM_SETTINGCHANGE -> WM_WININICHANGE
    public const int WM_SETTINGCHANGE = WinApiConstants.WM_WININICHANGE;

    /// WM_RBUTTONDBLCLK -> 0x0206
    public const int WM_RBUTTONDBLCLK = 518;

    /// WM_QUERYDRAGICON -> 0x0037
    public const int WM_QUERYDRAGICON = 55;

    /// WM_NCXBUTTONDOWN -> 0x00AB
    public const int WM_NCXBUTTONDOWN = 171;

    /// WM_NCRBUTTONDOWN -> 0x00A4
    public const int WM_NCRBUTTONDOWN = 164;

    /// WM_NCMBUTTONDOWN -> 0x00A7
    public const int WM_NCMBUTTONDOWN = 167;

    /// WM_NCLBUTTONDOWN -> 0x00A1
    public const int WM_NCLBUTTONDOWN = 161;

    /// WM_MOUSEACTIVATE -> 0x0021
    public const int WM_MOUSEACTIVATE = 33;

    /// WM_MENURBUTTONUP -> 0x0122
    public const int WM_MENURBUTTONUP = 290;

    /// WM_MENUGETOBJECT -> 0x0124
    public const int WM_MENUGETOBJECT = 292;

    /// WM_MBUTTONDBLCLK -> 0x0209
    public const int WM_MBUTTONDBLCLK = 521;

    /// WM_LBUTTONDBLCLK -> 0x0203
    public const int WM_LBUTTONDBLCLK = 515;

    /// WM_INITMENUPOPUP -> 0x0117
    public const int WM_INITMENUPOPUP = 279;

    /// WM_HANDHELDFIRST -> 0x0358
    public const int WM_HANDHELDFIRST = 856;

    /// WM_GETTEXTLENGTH -> 0x000E
    public const int WM_GETTEXTLENGTH = 14;

    /// WM_GETMINMAXINFO -> 0x0024
    public const int WM_GETMINMAXINFO = 36;

    /// WM_ENTERSIZEMOVE -> 0x0231
    public const int WM_ENTERSIZEMOVE = 561;

    /// WM_ENTERMENULOOP -> 0x0211
    public const int WM_ENTERMENULOOP = 529;

    /// WM_DRAWCLIPBOARD -> 0x0308
    public const int WM_DRAWCLIPBOARD = 776;

    /// WM_DISPLAYCHANGE -> 0x007E
    public const int WM_DISPLAYCHANGE = 126;

    /// WM_DEVMODECHANGE -> 0x001B
    public const int WM_DEVMODECHANGE = 27;

    /// WM_DDE_TERMINATE -> (WM_DDE_FIRST+1)
    public const int WM_DDE_TERMINATE = (WinApiConstants.WM_DDE_FIRST + 1);

    /// WM_CHILDACTIVATE -> 0x0022
    public const int WM_CHILDACTIVATE = 34;

    /// WM_CHANGEUISTATE -> 0x0127
    public const int WM_CHANGEUISTATE = 295;

    /// WM_CHANGECBCHAIN -> 0x030D
    public const int WM_CHANGECBCHAIN = 781;

    /// WM_CANCELJOURNAL -> 0x004B
    public const int WM_CANCELJOURNAL = 75;

    /// WM_WININICHANGE -> 0x001A
    public const int WM_WININICHANGE = 26;

    /// WM_THEMECHANGED -> 0x031A
    public const int WM_THEMECHANGED = 794;

    /// WM_TABLET_FIRST -> 0x02c0
    public const int WM_TABLET_FIRST = 704;

    /// WM_STYLECHANGED -> 0x007D
    public const int WM_STYLECHANGED = 125;

    /// WM_RENDERFORMAT -> 0x0305
    public const int WM_RENDERFORMAT = 773;

    /// WM_QUERYUISTATE -> 0x0129
    public const int WM_QUERYUISTATE = 297;

    /// WM_PARENTNOTIFY -> 0x0210
    public const int WM_PARENTNOTIFY = 528;

    /// WM_NOTIFYFORMAT -> 0x0055
    public const int WM_NOTIFYFORMAT = 85;

    /// WM_NCMOUSELEAVE -> 0x02A2
    public const int WM_NCMOUSELEAVE = 674;

    /// WM_NCMOUSEHOVER -> 0x02A0
    public const int WM_NCMOUSEHOVER = 672;

    /// WM_MDIGETACTIVE -> 0x0229
    public const int WM_MDIGETACTIVE = 553;

    /// WM_HANDHELDLAST -> 0x035F
    public const int WM_HANDHELDLAST = 863;

    /// WM_EXITSIZEMOVE -> 0x0232
    public const int WM_EXITSIZEMOVE = 562;

    /// WM_EXITMENULOOP -> 0x0212
    public const int WM_EXITMENULOOP = 530;

    /// WM_DEVICECHANGE -> 0x0219
    public const int WM_DEVICECHANGE = 537;

    /// WM_DDE_UNADVISE -> (WM_DDE_FIRST+3)
    public const int WM_DDE_UNADVISE = (WinApiConstants.WM_DDE_FIRST + 3);

    /// WM_DDE_INITIATE -> (WM_DDE_FIRST)
    public const int WM_DDE_INITIATE = WinApiConstants.WM_DDE_FIRST;

    /// WM_CTLCOLOREDIT -> 0x0133
    public const int WM_CTLCOLOREDIT = 307;

    /// WM_XBUTTONDOWN -> 0x020B
    public const int WM_XBUTTONDOWN = 523;

    /// WM_USERCHANGED -> 0x0054
    public const int WM_USERCHANGED = 84;

    /// WM_TABLET_LAST -> 0x02df
    public const int WM_TABLET_LAST = 735;

    /// WM_SYSDEADCHAR -> 0x0107
    public const int WM_SYSDEADCHAR = 263;

    /// WM_RBUTTONDOWN -> 0x0204
    public const int WM_RBUTTONDOWN = 516;

    /// WM_PRINTCLIENT -> 0x0318
    public const int WM_PRINTCLIENT = 792;

    /// WM_PENWINFIRST -> 0x0380
    public const int WM_PENWINFIRST = 896;

    /// WM_NCXBUTTONUP -> 0x00AC
    public const int WM_NCXBUTTONUP = 172;

    /// WM_NCRBUTTONUP -> 0x00A5
    public const int WM_NCRBUTTONUP = 165;

    /// WM_NCMOUSEMOVE -> 0x00A0
    public const int WM_NCMOUSEMOVE = 160;

    /// WM_NCMBUTTONUP -> 0x00A8
    public const int WM_NCMBUTTONUP = 168;

    /// WM_NCLBUTTONUP -> 0x00A2
    public const int WM_NCLBUTTONUP = 162;

    /// WM_MENUCOMMAND -> 0x0126
    public const int WM_MENUCOMMAND = 294;

    /// WM_MEASUREITEM -> 0x002C
    public const int WM_MEASUREITEM = 44;

    /// WM_MDIMAXIMIZE -> 0x0225
    public const int WM_MDIMAXIMIZE = 549;

    /// WM_MDIACTIVATE -> 0x0222
    public const int WM_MDIACTIVATE = 546;

    /// WM_MBUTTONDOWN -> 0x0207
    public const int WM_MBUTTONDOWN = 519;

    /// WM_LBUTTONDOWN -> 0x0201
    public const int WM_LBUTTONDOWN = 513;

    /// WM_IME_REQUEST -> 0x0288
    public const int WM_IME_REQUEST = 648;

    /// WM_IME_KEYLAST -> 0x010F
    public const int WM_IME_KEYLAST = 271;

    /// WM_IME_KEYDOWN -> 0x0290
    public const int WM_IME_KEYDOWN = 656;

    /// WM_IME_CONTROL -> 0x0283
    public const int WM_IME_CONTROL = 643;

    /// WM_DDE_REQUEST -> (WM_DDE_FIRST+6)
    public const int WM_DDE_REQUEST = (WinApiConstants.WM_DDE_FIRST + 6);

    /// WM_DDE_EXECUTE -> (WM_DDE_FIRST+8)
    public const int WM_DDE_EXECUTE = (WinApiConstants.WM_DDE_FIRST + 8);

    /// WM_CTLCOLORDLG -> 0x0136
    public const int WM_CTLCOLORDLG = 310;

    /// WM_CTLCOLORBTN -> 0x0135
    public const int WM_CTLCOLORBTN = 309;

    /// WM_CONTEXTMENU -> 0x007B
    public const int WM_CONTEXTMENU = 123;

    /// WM_COMPAREITEM -> 0x0039
    public const int WM_COMPAREITEM = 57;

    /// WM_ACTIVATEAPP -> 0x001C
    public const int WM_ACTIVATEAPP = 28;

    /// WM_VKEYTOITEM -> 0x002E
    public const int WM_VKEYTOITEM = 46;

    /// WM_TIMECHANGE -> 0x001E
    public const int WM_TIMECHANGE = 30;

    /// WM_SYSKEYDOWN -> 0x0104
    public const int WM_SYSKEYDOWN = 260;

    /// WM_SYSCOMMAND -> 0x0112
    public const int WM_SYSCOMMAND = 274;

    /// WM_SHOWWINDOW -> 0x0018
    public const int WM_SHOWWINDOW = 24;

    /// WM_PENWINLAST -> 0x038F
    public const int WM_PENWINLAST = 911;

    /// WM_NEXTDLGCTL -> 0x0028
    public const int WM_NEXTDLGCTL = 40;

    /// WM_NCCALCSIZE -> 0x0083
    public const int WM_NCCALCSIZE = 131;

    /// WM_NCACTIVATE -> 0x0086
    public const int WM_NCACTIVATE = 134;

    /// WM_MOUSEWHEEL -> 0x020A
    public const int WM_MOUSEWHEEL = 522;

    /// WM_MOUSELEAVE -> 0x02A3
    public const int WM_MOUSELEAVE = 675;

    /// WM_MOUSEHOVER -> 0x02A1
    public const int WM_MOUSEHOVER = 673;

    /// WM_MOUSEFIRST -> 0x0200
    public const int WM_MOUSEFIRST = 512;

    /// WM_MENUSELECT -> 0x011F
    public const int WM_MENUSELECT = 287;

    /// WM_MDISETMENU -> 0x0230
    public const int WM_MDISETMENU = 560;

    /// WM_MDIRESTORE -> 0x0223
    public const int WM_MDIRESTORE = 547;

    /// WM_MDIDESTROY -> 0x0221
    public const int WM_MDIDESTROY = 545;

    /// WM_MDICASCADE -> 0x0227
    public const int WM_MDICASCADE = 551;

    /// WM_INITDIALOG -> 0x0110
    public const int WM_INITDIALOG = 272;

    /// WM_IME_SELECT -> 0x0285
    public const int WM_IME_SELECT = 645;

    /// WM_IME_NOTIFY -> 0x0282
    public const int WM_IME_NOTIFY = 642;

    /// WM_GETDLGCODE -> 0x0087
    public const int WM_GETDLGCODE = 135;

    /// WM_FONTCHANGE -> 0x001D
    public const int WM_FONTCHANGE = 29;

    /// WM_ERASEBKGND -> 0x0014
    public const int WM_ERASEBKGND = 20;

    /// WM_ENDSESSION -> 0x0016
    public const int WM_ENDSESSION = 22;

    /// WM_DELETEITEM -> 0x002D
    public const int WM_DELETEITEM = 45;

    /// WM_DDE_ADVISE -> (WM_DDE_FIRST+2)
    public const int WM_DDE_ADVISE = (WinApiConstants.WM_DDE_FIRST + 2);

    /// WM_COMPACTING -> 0x0041
    public const int WM_COMPACTING = 65;

    /// WM_COMMNOTIFY -> 0x0044
    public const int WM_COMMNOTIFY = 68;

    /// WM_CHARTOITEM -> 0x002F
    public const int WM_CHARTOITEM = 47;

    /// WM_CANCELMODE -> 0x001F
    public const int WM_CANCELMODE = 31;

    /// WM_APPCOMMAND -> 0x0319
    public const int WM_APPCOMMAND = 793;

    /// WM_XBUTTONUP -> 0x020C
    public const int WM_XBUTTONUP = 524;

    /// WM_SYNCPAINT -> 0x0088
    public const int WM_SYNCPAINT = 136;

    /// WM_SETREDRAW -> 0x000B
    public const int WM_SETREDRAW = 11;

    /// WM_SETHOTKEY -> 0x0032
    public const int WM_SETHOTKEY = 50;

    /// WM_SETCURSOR -> 0x0020
    public const int WM_SETCURSOR = 32;

    /// WM_RBUTTONUP -> 0x0205
    public const int WM_RBUTTONUP = 517;

    /// WM_QUEUESYNC -> 0x0023
    public const int WM_QUEUESYNC = 35;

    /// WM_QUERYOPEN -> 0x0013
    public const int WM_QUERYOPEN = 19;

    /// WM_PAINTICON -> 0x0026
    public const int WM_PAINTICON = 38;

    /// WM_NCHITTEST -> 0x0084
    public const int WM_NCHITTEST = 132;

    /// WM_NCDESTROY -> 0x0082
    public const int WM_NCDESTROY = 130;

    /// WM_MOUSEMOVE -> 0x0200
    public const int WM_MOUSEMOVE = 512;

    /// WM_MOUSELAST -> 0x020D
    public const int WM_MOUSELAST = 525;

    /// WM_MDICREATE -> 0x0220
    public const int WM_MDICREATE = 544;

    /// WM_MBUTTONUP -> 0x0208
    public const int WM_MBUTTONUP = 520;

    /// WM_LBUTTONUP -> 0x0202
    public const int WM_LBUTTONUP = 514;

    /// WM_KILLFOCUS -> 0x0008
    public const int WM_KILLFOCUS = 8;

    /// WM_IME_KEYUP -> 0x0291
    public const int WM_IME_KEYUP = 657;

    /// WM_GETOBJECT -> 0x003D
    public const int WM_GETOBJECT = 61;

    /// WM_GETHOTKEY -> 0x0033
    public const int WM_GETHOTKEY = 51;

    /// WM_ENTERIDLE -> 0x0121
    public const int WM_ENTERIDLE = 289;

    /// WM_DROPFILES -> 0x0233
    public const int WM_DROPFILES = 563;

    /// WM_DDE_FIRST -> 0x03E0
    public const int WM_DDE_FIRST = 992;

    /// WM_SYSKEYUP -> 0x0105
    public const int WM_SYSKEYUP = 261;

    /// WM_SETFOCUS -> 0x0007
    public const int WM_SETFOCUS = 7;

    /// WM_NEXTMENU -> 0x0213
    public const int WM_NEXTMENU = 531;

    /// WM_NCCREATE -> 0x0081
    public const int WM_NCCREATE = 129;

    /// WM_MENUDRAG -> 0x0123
    public const int WM_MENUDRAG = 291;

    /// WM_MENUCHAR -> 0x0120
    public const int WM_MENUCHAR = 288;

    /// WM_KEYFIRST -> 0x0100
    public const int WM_KEYFIRST = 256;

    /// WM_INITMENU -> 0x0116
    public const int WM_INITMENU = 278;

    /// WM_IME_CHAR -> 0x0286
    public const int WM_IME_CHAR = 646;

    /// WM_DRAWITEM -> 0x002B
    public const int WM_DRAWITEM = 43;

    /// WM_DEADCHAR -> 0x0103
    public const int WM_DEADCHAR = 259;

    /// WM_DDE_POKE -> (WM_DDE_FIRST+7)
    public const int WM_DDE_POKE = (WinApiConstants.WM_DDE_FIRST + 7);

    /// WM_DDE_LAST -> (WM_DDE_FIRST+8)
    public const int WM_DDE_LAST = (WinApiConstants.WM_DDE_FIRST + 8);

    /// WM_DDE_DATA -> (WM_DDE_FIRST+5)
    public const int WM_DDE_DATA = (WinApiConstants.WM_DDE_FIRST + 5);

    /// WM_COPYDATA -> 0x004A
    public const int WM_COPYDATA = 74;

    /// WM_AFXFIRST -> 0x0360
    public const int WM_AFXFIRST = 864;

    /// WM_ACTIVATE -> 0x0006
    public const int WM_ACTIVATE = 6;

    /// WM_VSCROLL -> 0x0115
    public const int WM_VSCROLL = 277;

    /// WM_UNICHAR -> 0x0109
    public const int WM_UNICHAR = 265;

    /// WM_SYSCHAR -> 0x0106
    public const int WM_SYSCHAR = 262;

    /// WM_SETTEXT -> 0x000C
    public const int WM_SETTEXT = 12;

    /// WM_SETICON -> 0x0080
    public const int WM_SETICON = 128;

    /// WM_SETFONT -> 0x0030
    public const int WM_SETFONT = 48;

    /// WM_NCPAINT -> 0x0085
    public const int WM_NCPAINT = 133;

    /// WM_MDITILE -> 0x0226
    public const int WM_MDITILE = 550;

    /// WM_MDINEXT -> 0x0224
    public const int WM_MDINEXT = 548;

    /// WM_KEYLAST -> 0x0109
    public const int WM_KEYLAST = 265;

    /// WM_KEYDOWN -> 0x0100
    public const int WM_KEYDOWN = 256;

    /// WM_HSCROLL -> 0x0114
    public const int WM_HSCROLL = 276;

    /// WM_GETTEXT -> 0x000D
    public const int WM_GETTEXT = 13;

    /// WM_GETICON -> 0x007F
    public const int WM_GETICON = 127;

    /// WM_GETFONT -> 0x0031
    public const int WM_GETFONT = 49;

    /// WM_DESTROY -> 0x0002
    public const int WM_DESTROY = 2;

    /// WM_DDE_ACK -> (WM_DDE_FIRST+4)
    public const int WM_DDE_ACK = (WinApiConstants.WM_DDE_FIRST + 4);

    /// WM_COMMAND -> 0x0111
    public const int WM_COMMAND = 273;

    /// WM_AFXLAST -> 0x037F
    public const int WM_AFXLAST = 895;

    /// WM_SIZING -> 0x0214
    public const int WM_SIZING = 532;

    /// WM_NOTIFY -> 0x004E
    public const int WM_NOTIFY = 78;

    /// WM_MOVING -> 0x0216
    public const int WM_MOVING = 534;

    /// WM_HOTKEY -> 0x0312
    public const int WM_HOTKEY = 786;

    /// WM_ENABLE -> 0x000A
    public const int WM_ENABLE = 10;

    /// WM_CREATE -> 0x0001
    public const int WM_CREATE = 1;

    /// WM_TIMER -> 0x0113
    public const int WM_TIMER = 275;

    /// WM_TCARD -> 0x0052
    public const int WM_TCARD = 82;

    /// WM_PRINT -> 0x0317
    public const int WM_PRINT = 791;

    /// WM_POWER -> 0x0048
    public const int WM_POWER = 72;

    /// WM_PASTE -> 0x0302
    public const int WM_PASTE = 770;

    /// WM_PAINT -> 0x000F
    public const int WM_PAINT = 15;

    /// WM_KEYUP -> 0x0101
    public const int WM_KEYUP = 257;

    /// WM_INPUT -> 0x00FF
    public const int WM_INPUT = 255;

    /// WM_CLOSE -> 0x0010
    public const int WM_CLOSE = 16;

    /// WM_CLEAR -> 0x0303
    public const int WM_CLEAR = 771;

    /// WM_USER -> 0x0400
    public const int WM_USER = 1024;

    /// WM_UNDO -> 0x0304
    public const int WM_UNDO = 772;

    /// WM_SIZE -> 0x0005
    public const int WM_SIZE = 5;

    /// WM_QUIT -> 0x0012
    public const int WM_QUIT = 18;

    /// WM_NULL -> 0x0000
    public const int WM_NULL = 0;

    /// WM_MOVE -> 0x0003
    public const int WM_MOVE = 3;

    /// WM_HELP -> 0x0053
    public const int WM_HELP = 83;

    /// WM_CHAR -> 0x0102
    public const int WM_CHAR = 258;

    /// WM_CUT -> 0x0300
    public const int WM_CUT = 768;

    /// WM_APP -> 0x8000
    public const int WM_APP = 32768;

    /// WM_COPY -> 0x0301
    public const int WM_COPY = 769;

    public const int WM_MOUSEHWHEEL = 0x020E;

    /// MOUSEEVENTF_VIRTUALDESK -> 0x4000
    public const int MOUSEEVENTF_VIRTUALDESK = 16384;

    /// MOUSEEVENTF_MIDDLEDOWN -> 0x0020
    public const int MOUSEEVENTF_MIDDLEDOWN = 32;

    /// MOUSEEVENTF_RIGHTDOWN -> 0x0008
    public const int MOUSEEVENTF_RIGHTDOWN = 8;

    /// MOUSEEVENTF_MIDDLEUP -> 0x0040
    public const int MOUSEEVENTF_MIDDLEUP = 64;

    /// MOUSEEVENTF_LEFTDOWN -> 0x0002
    public const int MOUSEEVENTF_LEFTDOWN = 2;

    /// MOUSEEVENTF_ABSOLUTE -> 0x8000
    public const int MOUSEEVENTF_ABSOLUTE = 32768;

    /// MOUSEEVENTF_RIGHTUP -> 0x0010
    public const int MOUSEEVENTF_RIGHTUP = 16;

    /// MOUSEEVENTF_LEFTUP -> 0x0004
    public const int MOUSEEVENTF_LEFTUP = 4;

    /// MOUSEEVENTF_XDOWN -> 0x0080
    public const int MOUSEEVENTF_XDOWN = 128;

    /// MOUSEEVENTF_WHEEL -> 0x0800
    public const int MOUSEEVENTF_WHEEL = 2048;

    /// MOUSEEVENTF_XUP -> 0x0100
    public const int MOUSEEVENTF_XUP = 256;

    /// MOUSEEVENTF_MOVE -> 0x0001
    public const int MOUSEEVENTF_MOVE = 1;


    /// ESB_DISABLE_RIGHT -> 0x0002
    public const int ESB_DISABLE_RIGHT = 2;

    /// IN_CLASSB_NSHIFT -> 16
    public const int IN_CLASSB_NSHIFT = 16;

    /// ESB_DISABLE_RTDN -> ESB_DISABLE_RIGHT
    public const int ESB_DISABLE_RTDN = WinApiConstants.ESB_DISABLE_RIGHT;

    /// ESB_DISABLE_LTUP -> ESB_DISABLE_LEFT
    public const int ESB_DISABLE_LTUP = WinApiConstants.ESB_DISABLE_LEFT;

    /// ESB_DISABLE_LEFT -> 0x0001
    public const int ESB_DISABLE_LEFT = 1;

    /// ESB_DISABLE_DOWN -> 0x0002
    public const int ESB_DISABLE_DOWN = 2;

    /// ESB_DISABLE_BOTH -> 0x0003
    public const int ESB_DISABLE_BOTH = 3;

    /// ESB_ENABLE_BOTH -> 0x0000
    public const int ESB_ENABLE_BOTH = 0;

    /// IN_CLASSB_HOST -> 0x0000ffff
    public const int IN_CLASSB_HOST = 65535;

    /// ESB_DISABLE_UP -> 0x0001
    public const int ESB_DISABLE_UP = 1;

    /// IN_CLASSB_NET -> 0xffff0000
    public const int IN_CLASSB_NET = -65536;

    /// IN_CLASSB_MAX -> 65536
    public const int IN_CLASSB_MAX = 65536;

    /// SB_THUMBPOSITION -> 4
    public const int SB_THUMBPOSITION = 4;

    /// SB_PREMULT_ALPHA -> 0x00000004
    public const int SB_PREMULT_ALPHA = 4;

    /// SB_PIXEL_ALPHA -> 0x00000002
    public const int SB_PIXEL_ALPHA = 2;

    /// SB_CONST_ALPHA -> 0x00000001
    public const int SB_CONST_ALPHA = 1;

    /// SB_THUMBTRACK -> 5
    public const int SB_THUMBTRACK = 5;

    /// SB_PAGERIGHT -> 3
    public const int SB_PAGERIGHT = 3;

    /// SB_LINERIGHT -> 1
    public const int SB_LINERIGHT = 1;

    /// SB_GRAD_RECT -> 0x00000010
    public const int SB_GRAD_RECT = 16;

    /// SB_ENDSCROLL -> 8
    public const int SB_ENDSCROLL = 8;

    /// SB_PAGELEFT -> 2
    public const int SB_PAGELEFT = 2;

    /// SB_PAGEDOWN -> 3
    public const int SB_PAGEDOWN = 3;

    /// SB_LINELEFT -> 0
    public const int SB_LINELEFT = 0;

    /// SB_LINEDOWN -> 1
    public const int SB_LINEDOWN = 1;

    /// SB_GRAD_TRI -> 0x00000020
    public const int SB_GRAD_TRI = 32;

    /// SB_PAGEUP -> 2
    public const int SB_PAGEUP = 2;

    /// SB_LINEUP -> 0
    public const int SB_LINEUP = 0;

    /// SB_BOTTOM -> 7
    public const int SB_BOTTOM = 7;

    /// SB_RIGHT -> 7
    public const int SB_RIGHT = 7;

    /// SB_VERT -> 1
    public const int SB_VERT = 1;

    /// SB_NONE -> 0x00000000
    public const int SB_NONE = 0;

    /// SB_LEFT -> 6
    public const int SB_LEFT = 6;

    /// SB_HORZ -> 0
    public const int SB_HORZ = 0;

    /// SB_BOTH -> 3
    public const int SB_BOTH = 3;

    /// SB_CTL -> 2
    public const int SB_CTL = 2;

    /// SB_TOP -> 6
    public const int SB_TOP = 6;

    /// X3_D_WH_INST_WORD_POS_X -> 24
    public const int X3_D_WH_INST_WORD_POS_X = 24;

    /// X3_D_WH_SIGN_VAL_POS_X -> 0
    public const int X3_D_WH_SIGN_VAL_POS_X = 0;

    /// X3_D_WH_INST_WORD_X -> 3
    public const int X3_D_WH_INST_WORD_X = 3;

    /// X3_D_WH_SIZE_X -> 3
    public const int X3_D_WH_SIZE_X = 3;

    /// WH_JOURNALPLAYBACK -> 1
    public const int WH_JOURNALPLAYBACK = 1;

    /// WH_FOREGROUNDIDLE -> 11
    public const int WH_FOREGROUNDIDLE = 11;

    /// WH_CALLWNDPROCRET -> 12
    public const int WH_CALLWNDPROCRET = 12;

    /// WH_JOURNALRECORD -> 0
    public const int WH_JOURNALRECORD = 0;

    /// WH_SYSMSGFILTER -> 6
    public const int WH_SYSMSGFILTER = 6;

    /// WH_KEYBOARD_LL -> 13
    public const int WH_KEYBOARD_LL = 13;

    /// WH_CALLWNDPROC -> 4
    public const int WH_CALLWNDPROC = 4;

    /// WH_GETMESSAGE -> 3
    public const int WH_GETMESSAGE = 3;

    /// WH_MSGFILTER -> (-1)
    public const int WH_MSGFILTER = -1;

    /// WH_MOUSE_LL -> 14
    public const int WH_MOUSE_LL = 14;

    /// WH_KEYBOARD -> 2
    public const int WH_KEYBOARD = 2;

    /// WH_MINHOOK -> WH_MIN
    public const int WH_MINHOOK = WinApiConstants.WH_MIN;

    /// WH_MAXHOOK -> WH_MAX
    public const int WH_MAXHOOK = WinApiConstants.WH_MAX;

    /// WH_SHELL -> 10
    public const int WH_SHELL = 10;

    /// WH_MOUSE -> 7
    public const int WH_MOUSE = 7;

    /// WH_DEBUG -> 9
    public const int WH_DEBUG = 9;

    /// WH_MAX -> 14
    public const int WH_MAX = 14;

    /// WH_CBT -> 5
    public const int WH_CBT = 5;

    /// WH_MIN -> (-1)
    public const int WH_MIN = -1;
}
