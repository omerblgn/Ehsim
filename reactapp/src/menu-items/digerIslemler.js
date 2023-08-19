// assets
import { IconUsers, IconPackage, IconReportMoney } from '@tabler/icons';

// constant
const icons = { IconUsers, IconPackage, IconReportMoney };

// ==============================|| DASHBOARD MENU ITEMS ||============================== //

const digerIslemler = {
    id: 'digerIslemler',
    title: 'Diğer İşlemler',
    type: 'group',
    children: [
        {
            id: 'musteriler',
            title: 'Müşteriler',
            type: 'collapse',
            icon: icons.IconUsers,

            children: [
                {
                    id: 'musteriler',
                    title: 'Müşteri Listesi',
                    type: 'item',
                    url: '/digerIslemler/musteriler'
                },
                {
                    id: 'musteri-ekle',
                    title: 'Müşteri Ekle',
                    type: 'item',
                    url: '/digerIslemler/musteri-ekle'
                }
            ]
        },
        {
            id: 'urunler',
            title: 'Ürünler',
            type: 'collapse',
            icon: icons.IconPackage,
            children: [
                {
                    id: 'urunler',
                    title: 'Ürün Listesi',
                    type: 'item',
                    url: '/digerIslemler/urunler'
                },
                {
                    id: 'urun-ekle',
                    title: 'Ürün Ekle',
                    type: 'item',
                    url: '/digerIslemler/urun-ekle'
                }
            ]
        },
        {
            id: 'teklifler',
            title: 'Teklifler',
            type: 'collapse',
            icon: icons.IconReportMoney,
            children: [
                {
                    id: 'tekliflerim',
                    title: 'Tekliflerim',
                    type: 'item',
                    url: '/digerIslemler/tekliflerim'
                },
                {
                    id: 'gelen-teklifler',
                    title: 'Gelen Teklifler',
                    type: 'item',
                    url: '/digerIslemler/gelen-teklifler'
                }
            ]
        }
    ]
};

export default digerIslemler;
