// assets
import { IconUsers, IconPackage, IconReportMoney, IconCategory } from '@tabler/icons';

// constant
const icons = { IconUsers, IconPackage, IconReportMoney, IconCategory };

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
            id: 'kategoriler',
            title: 'Kategoriler',
            type: 'collapse',
            icon: icons.IconCategory,

            children: [
                {
                    id: 'kategoriler',
                    title: 'Kategori Listesi',
                    type: 'item',
                    url: '/digerIslemler/kategoriler'
                },
                {
                    id: 'kategori-ekle',
                    title: 'Kategori Ekle',
                    type: 'item',
                    url: '/digerIslemler/kategori-ekle'
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
                    id: 'teklif-olustur',
                    title: 'Teklif Oluştur',
                    type: 'item',
                    url: '/digerIslemler/teklif-olustur'
                },
                {
                    id: 'tekliflerim',
                    title: 'Tekliflerim',
                    type: 'item',
                    url: '/digerIslemler/tekliflerim'
                }
            ]
        }
    ]
};

export default digerIslemler;
