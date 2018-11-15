namespace ConfigDatas
{
    public interface IHitDamage
    {
        int Value { get;}//�˺�ֵ
        bool UseSource { get; set; }//�Ƿ����ӷ���
        int SourceValue { get; }

        bool SetPDamageRate(double rate);

        bool SetMDamageRate(double rate);

        bool AddPDamage(double damage);

        bool AddMDamage(double damage);
    }
}