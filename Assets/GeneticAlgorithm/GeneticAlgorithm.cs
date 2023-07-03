using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

///<summary>
/// 1. 適当な遺伝子配列をつくる
/// 2. その遺伝子配列の評価を行う
/// 3. ルーレット選択によって値を決める
/// 4. 一点交叉で遺伝子配列の一部を入れ替える
/// 5. 突然変異で遺伝子配列の一部を変更
/// 6. 評価を行い、以前の評価値より高い遺伝子配列を残す
/// 7. 3 ~ 6 を繰り返し、指定した世代まで行う
/// </summary>



class GeneticAlgorithm : MonoBehaviour
{
    // おこづかい（所持金）
    static readonly int MaxPrice = 1000;
    // 商品1つあたりの最大価格
    static readonly int MaxItemPrice = 100;
    // お店にある商品の数
    static readonly int ItemCount = 20;
    // 持っていけるお菓子の数 (遺伝子列)
    static readonly int GeneData = 10;
    // 1世代あたりの個体数
    static readonly int GeneCount = 8;
    // GAを終了する世代
    static readonly int EndGenCount = 50;
    // 何世代ごとに点数を書き出すか
    static readonly int ShowGen = 1;
    // 交叉率
    static readonly double CrosRate = 0.9;
    // 突然変異が起こる確率
    static readonly double MutaRate = 0.8;

    System.Random rand = new System.Random();
    

    void Start()
    {
        Algorithm();
    }




    async void Algorithm()
    {
        int geneCounter = 0; //世代のを数えるやつ
        int[] price = new int[ItemCount];  //商品の値段配列
        int[,] gene = new int[GeneCount, GeneData]; //その世代の個体群 [1世代の個体数, 各個体の遺伝子列の長さ] プリミティブな値は商品の値段配列の中からどれを選んだかのインデックス(このインデックスが遺伝子)

        int ruletNum = 0; 
        
        double[] results = new double[GeneCount]; //１世代分のの全個体の成績を格納する
        double topResult; //各世代の results[] の中のトップスコアのみを集める
        int[] elite = new int[GeneData]; //１世代分ののトップスコアの遺伝子配列のみ集める配列


        //各商品の価格を決める
        for (int i = 0; i < ItemCount; i++)
        {
            price[i] = rand.Next(10, MaxItemPrice);
        }

        //第1世代目（適当な遺伝子を与える）
        for (int i = 0; i < GeneCount; i++)
        {
            for (int j = 0; j < GeneData; j++)
            {
                gene[i, j] = rand.Next(0, ItemCount);
            }
        }




        //ここから進化開始（世代交代数初期化）
        while (true)
        {
            //その世代の個体の成績を総和
            for (int i = 0; i < GeneCount; i++)
            {
                //その個体の成績初期化
                results[i] = 0.0;

                for (int j = 0; j < GeneData; j++)
                {
                    results[i] += price[gene[i, j]]; //今回の世代の全個体の成績格納
                }
                // 所持金を超えたら0点
                if (results[i] > MaxPrice)
                {
                    results[i] = 0.0;
                }
            }



            
            //初期値として優秀な個体番号を0番目と仮定
            int eliteNum = 0;
            //優秀個体と仮定した0番目個体のもつ成績を保持
            topResult = results[eliteNum];
            
            //初期の優秀な個体を超える個体を見つける
            for (int i = 1; i < GeneCount; i++)
            {
                if (topResult < results[i])
                {
                    eliteNum = i;
                    topResult = results[i];
                }
            }
            //優秀な個体の遺伝子情報を保持
            for (int i = 0; i < GeneData; i++)
            {
                elite[i] = gene[eliteNum, i];
            }




            double total = 0.0; //その世代の個体の成績総和
            for (int i = 0; i < GeneCount; i++)
            {
                total += results[i];
            }
            
            int[,] gene_Temporary = new int[GeneCount, GeneData]; //個体群一時保存用のバッファ
            //0～1 の値に成績の総和を掛ける
            for (int i = 0; i < GeneCount; i++)
            {
                double ruletCf = rand.NextDouble() * total; //
                double ruletValue = 0.0; //

                for (int j = 0; j < GeneCount; j++)
                {
                    ruletValue += results[j];

                    if (ruletValue > ruletCf)
                    {
                        ruletNum = j;
                        break;
                    }
                }
                // 仮の個体群配列に選択された個体を入れる
                for (int j = 0; j < GeneData; j++)
                {
                    gene_Temporary[i, j] = gene[ruletNum, j];
                }
            }
            
            for (int i = 0; i < GeneCount; i++)
            {
                for (int j = 0; j < GeneData; j++)
                {
                    gene[i, j] = gene_Temporary[i, j];
                }
            }



            
            //交叉開始
            for (int i = 0; i < GeneCount; i += 2)
            {
                //交叉確率の選定
                double dRet = rand.NextDouble();
                //交叉率よりも値が低い場合のみ交叉
                if (dRet < CrosRate)
                {
                    int pos = rand.Next(0, GeneData); //遺伝子配列のどの箇所まで入れ替えるか
                    //遺伝子の入れ替え
                    for (int j = pos; j < GeneData; j++)
                    {
                        int a = gene[i, j];
                        gene[i, j] = gene[i + 1, j];
                        gene[i + 1, j] = a;
                    }
                }
            }




            //突然変異開始
            for (int i = 0; i < GeneCount; i++)
            {
                for (int j = 0; j < GeneData; j++)
                {
                    //突然変異確率の選定
                    double dRet = rand.NextDouble();
                    //突然変異率よりも値が低い場合のみ突然変異
                    if (dRet < MutaRate)
                    {
                        gene[i, j] = rand.Next(ItemCount);
                    }
                }
            }




            // --------------------------- 進化終了 ---------------------------




            // 待機している前世代の優秀個体の番号を個体として戻す
            for (int i = 0; i < GeneData; i++)
            {
                gene[0, i] = elite[i];
            }


            

            geneCounter++;

            //現段階での状態を出力
            if (geneCounter % ShowGen == 0)
            {
                if (geneCounter == 1)
                {
                    Debug.Log($"世代\t評価点\t選んだ商品の価格(遺伝子情報)");
                }
                string tex = $"{geneCounter}\t{topResult}\t";
                for (int i = 0; i < GeneData; i++)
                {
                    tex += $"{price[gene[0, i]]}({gene[0, i]})\t";
                }
                Debug.Log(tex);
            }

            if (geneCounter == EndGenCount) break;
            await UniTask.Delay(TimeSpan.FromSeconds(1));
        }
    }
}