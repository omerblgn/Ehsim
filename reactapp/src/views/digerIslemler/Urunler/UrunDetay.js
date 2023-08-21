import { useParams } from 'react-router';

const UrunDetay = () => {
    const { id } = useParams();

    return <h1>Ürün Detay - {id}</h1>;
};

export default UrunDetay;
