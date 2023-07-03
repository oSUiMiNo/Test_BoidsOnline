using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

///<summary>
/// 1. �K���Ȉ�`�q�z�������
/// 2. ���̈�`�q�z��̕]�����s��
/// 3. ���[���b�g�I���ɂ���Ēl�����߂�
/// 4. ��_�����ň�`�q�z��̈ꕔ�����ւ���
/// 5. �ˑR�ψقň�`�q�z��̈ꕔ��ύX
/// 6. �]�����s���A�ȑO�̕]���l��荂����`�q�z����c��
/// 7. 3 ~ 6 ���J��Ԃ��A�w�肵������܂ōs��
/// </summary>



class GeneticAlgorithm : MonoBehaviour
{
    // �����Â����i�������j
    static readonly int MaxPrice = 1000;
    // ���i1������̍ő剿�i
    static readonly int MaxItemPrice = 100;
    // ���X�ɂ��鏤�i�̐�
    static readonly int ItemCount = 20;
    // �����Ă����邨�َq�̐� (��`�q��)
    static readonly int GeneData = 10;
    // 1���゠����̌̐�
    static readonly int GeneCount = 8;
    // GA���I�����鐢��
    static readonly int EndGenCount = 50;
    // �����ゲ�Ƃɓ_���������o����
    static readonly int ShowGen = 1;
    // ������
    static readonly double CrosRate = 0.9;
    // �ˑR�ψق��N����m��
    static readonly double MutaRate = 0.8;

    System.Random rand = new System.Random();
    

    void Start()
    {
        Algorithm();
    }




    async void Algorithm()
    {
        int geneCounter = 0; //����̂𐔂�����
        int[] price = new int[ItemCount];  //���i�̒l�i�z��
        int[,] gene = new int[GeneCount, GeneData]; //���̐���̌̌Q [1����̌̐�, �e�̂̈�`�q��̒���] �v���~�e�B�u�Ȓl�͏��i�̒l�i�z��̒�����ǂ��I�񂾂��̃C���f�b�N�X(���̃C���f�b�N�X����`�q)

        int ruletNum = 0; 
        
        double[] results = new double[GeneCount]; //�P���㕪�̂̑S�̂̐��т��i�[����
        double topResult; //�e����� results[] �̒��̃g�b�v�X�R�A�݂̂��W�߂�
        int[] elite = new int[GeneData]; //�P���㕪�̂̃g�b�v�X�R�A�̈�`�q�z��̂ݏW�߂�z��


        //�e���i�̉��i�����߂�
        for (int i = 0; i < ItemCount; i++)
        {
            price[i] = rand.Next(10, MaxItemPrice);
        }

        //��1����ځi�K���Ȉ�`�q��^����j
        for (int i = 0; i < GeneCount; i++)
        {
            for (int j = 0; j < GeneData; j++)
            {
                gene[i, j] = rand.Next(0, ItemCount);
            }
        }




        //��������i���J�n�i�����㐔�������j
        while (true)
        {
            //���̐���̌̂̐��т𑍘a
            for (int i = 0; i < GeneCount; i++)
            {
                //���̌̂̐��я�����
                results[i] = 0.0;

                for (int j = 0; j < GeneData; j++)
                {
                    results[i] += price[gene[i, j]]; //����̐���̑S�̂̐��ъi�[
                }
                // �������𒴂�����0�_
                if (results[i] > MaxPrice)
                {
                    results[i] = 0.0;
                }
            }



            
            //�����l�Ƃ��ėD�G�Ȍ̔ԍ���0�ԖڂƉ���
            int eliteNum = 0;
            //�D�G�̂Ɖ��肵��0�Ԗڌ̂̂����т�ێ�
            topResult = results[eliteNum];
            
            //�����̗D�G�Ȍ̂𒴂���̂�������
            for (int i = 1; i < GeneCount; i++)
            {
                if (topResult < results[i])
                {
                    eliteNum = i;
                    topResult = results[i];
                }
            }
            //�D�G�Ȍ̂̈�`�q����ێ�
            for (int i = 0; i < GeneData; i++)
            {
                elite[i] = gene[eliteNum, i];
            }




            double total = 0.0; //���̐���̌̂̐��ё��a
            for (int i = 0; i < GeneCount; i++)
            {
                total += results[i];
            }
            
            int[,] gene_Temporary = new int[GeneCount, GeneData]; //�̌Q�ꎞ�ۑ��p�̃o�b�t�@
            //0�`1 �̒l�ɐ��т̑��a���|����
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
                // ���̌̌Q�z��ɑI�����ꂽ�̂�����
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



            
            //�����J�n
            for (int i = 0; i < GeneCount; i += 2)
            {
                //�����m���̑I��
                double dRet = rand.NextDouble();
                //�����������l���Ⴂ�ꍇ�̂݌���
                if (dRet < CrosRate)
                {
                    int pos = rand.Next(0, GeneData); //��`�q�z��̂ǂ̉ӏ��܂œ���ւ��邩
                    //��`�q�̓���ւ�
                    for (int j = pos; j < GeneData; j++)
                    {
                        int a = gene[i, j];
                        gene[i, j] = gene[i + 1, j];
                        gene[i + 1, j] = a;
                    }
                }
            }




            //�ˑR�ψيJ�n
            for (int i = 0; i < GeneCount; i++)
            {
                for (int j = 0; j < GeneData; j++)
                {
                    //�ˑR�ψيm���̑I��
                    double dRet = rand.NextDouble();
                    //�ˑR�ψٗ������l���Ⴂ�ꍇ�̂ݓˑR�ψ�
                    if (dRet < MutaRate)
                    {
                        gene[i, j] = rand.Next(ItemCount);
                    }
                }
            }




            // --------------------------- �i���I�� ---------------------------




            // �ҋ@���Ă���O����̗D�G�̂̔ԍ����̂Ƃ��Ė߂�
            for (int i = 0; i < GeneData; i++)
            {
                gene[0, i] = elite[i];
            }


            

            geneCounter++;

            //���i�K�ł̏�Ԃ��o��
            if (geneCounter % ShowGen == 0)
            {
                if (geneCounter == 1)
                {
                    Debug.Log($"����\t�]���_\t�I�񂾏��i�̉��i(��`�q���)");
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