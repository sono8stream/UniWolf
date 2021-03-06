﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WodiLib.GameBasicDat
{
    public class GameBasicDat
    {
        /// 解析情報メモ
        /// 00-0A : ヘッダ 不変
        /// 0E : タイルサイズ 16or32or64
        /// 0F : キャラクター画像方向 4or8
        /// 10 : キャラクター移動可能方向 4or8
        /// 12 : FPS 30or60
        /// 13 : キャラクターの影 00 : 使わない，01 : 使う
        /// 14 : 音源設定，00 : ハードウェア音源，01 : ソフトウェア音源
        /// 15 : キャラクターアニメパターン　3or5
        /// 16 : デフォルトのキャラクター移動幅，00 : 0.5マス，01 : 1マス
        /// 17 : デフォルトの当たり判定，00 : 1x0.5マス，01 : 1x1マス
        /// 18 : 文章表示微調整，横方向の字詰め（ピクセル単位）
        /// 19 : 文章表示微調整，開業の間隔（ピクセル単位）
        /// 1A : 選択肢の改行間隔（ピクセル単位）
        /// 1C : フォントのアンチエイリアス，00:有り，01:無し，02:無し&倍角
        /// 1E-1F : キャラクターの移動速度->オート，イベント→主人公&仲間の順，表記される値/0.25(表記1なら4)
        /// 20 : ピクチャ拡大縮小時の描画方法，00 : くっきり&ガタガタ，01 : なめらか&ぼんやり
        /// 21 : ウィンドウ非アクティブ時の挙動，00 : 処理を停止，01 : 実行し続ける
        /// 22 : 2byte文字の言語設定，00 : 日本語，01 : その他の言語，設定した言語によって末尾のデータが異なりそう
        /// 23 : システム言語，00 : 日本語，01 : 英語
        /// 28 : ゲームタイトル，4byteが文字byte数，以降文字byte数続く，末尾は必ず00
        /// 謎の羅列×2，いずれもゲームタイトルと同様に文字列形式
        /// フォント名，4つまで，文字列形式
        /// 初期主人公画像のパス，文字列形式
        /// ゲーム名，文字列形式
        /// そこから14byte謎の羅列，その後↓
        /// キャラクターの移動速度，速度0-6について，それぞれ主人公&仲間→イベントの順に2byteずつ
        /// Game.exe動作バージョン特性，2byte
        /// ウィンドウ解像度，横x縦
        /// 主要なデータの解析終了
    }
}
