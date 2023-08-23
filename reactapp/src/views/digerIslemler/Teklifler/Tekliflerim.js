import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router';
import { useQuery, QueryClientProvider, QueryClient } from '@tanstack/react-query';
import { useMemo } from 'react';
import MaterialReactTable from 'material-react-table';
import { Box, IconButton, Tooltip } from '@mui/material';
import { Refresh as RefreshIcon, Delete as DeleteIcon, Print as PrintIcon } from '@mui/icons-material';
import { toast } from 'react-toastify';
import { useRef } from 'react';

const Example = () => {
    const sleep = (delay) => new Promise((resolve) => setTimeout(resolve, delay));
    const navigate = useNavigate();

    // const [teklifNo, setTeklifNo] = useState('');
    // const [teklifTarihi, setteklifTarihi] = useState('');
    // const [teklifSuresi, setTeklifSuresi] = useState('');
    // const [iskonto, setIskonto] = useState('');
    // const [toplamFiyatTRY, setToplamFiyatTRY] = useState('');
    // const [toplamFiyatUSD, setToplamFiyatUSD] = useState('');
    // const [toplamFiyatEUR, setToplamFiyatEUR] = useState('');
    // const [urunAdi, setUrunAdi] = useState('');
    // const [adet, setAdet] = useState('');
    const [res, setRes] = useState({});
    const [columnFilters, setColumnFilters] = useState([]);
    const [globalFilter, setGlobalFilter] = useState('');
    const [sorting, setSorting] = useState([]);
    const [pagination, setPagination] = useState({
        pageIndex: 0,
        pageSize: 10
    });

    const { data, isError, isFetching, isLoading, refetch } = useQuery({
        queryKey: ['table-data'],
        queryFn: async () => {
            var responseData;
            const FormData = require('form-data');
            let data = new FormData();
            data.append('pageSize', 0);
            data.append('pageIndex', 0);

            let config = {
                method: 'post',
                maxBodyLength: Infinity,
                url: 'http://localhost:5273/api/Teklif/GetTekliflerim',
                data: data
            };

            await axios
                .request(config)
                .then((response) => {
                    responseData = response.data.data;
                    setRes(responseData.list);
                    // console.log(responseData.list.teklifNo);
                    // setTeklifNo(responseData.list.teklifNo);
                    // setteklifTarihi(responseData.list.teklifTarihi);
                    // setTeklifSuresi(responseData.list.teklifSuresi);
                    // setIskonto(responseData.list.iskontoOrani);
                    // setToplamFiyatTRY(responseData.list.toplamFiyatTRY);
                    // setToplamFiyatUSD(responseData.list.toplamFiyatUSD);
                    // setToplamFiyatEUR(responseData.list.toplamFiyatEUR);
                    // responseData.list.map((teklif) =>
                    //     teklif.teklifItems.forEach((item) => {
                    //         setUrunAdi(item.urunAdi);
                    //         setAdet(item.adet);
                    //     })
                    // );
                })
                .catch((error) => {
                    console.log(error);
                });
            return responseData;
        },
        keepPreviousData: true
    });

    const columns = useMemo(
        () => [
            {
                accessorKey: 'teklifNo',
                header: 'Teklif No'
            },
            {
                accessorKey: 'teklifTarihi',
                header: 'Teklif Oluşturulma Tarihi',
                Cell: ({ cell }) => {
                    const formattedDate = new Date(cell.row.original.teklifTarihi).toLocaleString('tr-TR', {
                        year: 'numeric',
                        month: '2-digit',
                        day: '2-digit',
                        hour: '2-digit',
                        minute: '2-digit'
                    });
                    return <>{formattedDate}</>;
                }
            },
            {
                accessorKey: 'teklifSuresi',
                header: 'Teklif Süresi'
            },
            {
                accessorKey: 'iskontoOrani',
                header: 'İskonto Oranı'
            },
            {
                accessorKey: 'toplamFiyatTRY',
                header: 'Toplam Fiyat TRY'
            },
            {
                accessorKey: 'toplamFiyatUSD',
                header: 'Toplam Fiyat USD'
            },
            {
                accessorKey: 'toplamFiyatEUR',
                header: 'Toplam Fiyat EUR'
            }
        ],
        []
    );

    const deleteById = (id, ad, deger) => {
        toast.promise(deletePromise(id), {
            pending: ad + ' ürününe yapılan ' + deger + '₺ değerindeki teklif siliniyor.',
            success: ad + ' ürününe yapılan ' + deger + '₺ değerindeki teklif başarıyla silindi 👌',
            error: ad + ' ürününe yapılan ' + deger + '₺ değerindeki teklif silinirken hata oluştu 🤯'
        });
    };

    const deletePromise = (id) => {
        return new Promise(async (resolve, reject) => {
            let config = {
                method: 'get',
                maxBodyLength: Infinity,
                url: 'http://localhost:5273/api/Teklif/Delete',
                headers: {
                    'Content-Type': 'application/json',
                    Accept: 'text/plain'
                },
                params: {
                    id: id
                }
            };

            axios
                .request(config)
                .then(async (response) => {
                    console.log(JSON.stringify(response.data));
                    if (response.data.result) {
                        refetch();
                        resolve(response.data); // Başarılı sonuç durumunda Promise'ı çöz
                    } else {
                        reject(new Error('İşlem başarısız')); // Başarısız sonuç durumunda Promise'ı reddet
                    }
                })
                .catch((error) => {
                    console.log(error);
                    reject(error); // Hata durumunda Promise'ı reddet
                });
        });
    };

    const handlePrint = (row) => {
        res.map((t) => {
            if (t.id == row.id) {
                const printWindow = window.open('', '_blank');
                const formattedDate = new Date(row.teklifTarihi).toLocaleString('tr-TR', {
                    year: 'numeric',
                    month: '2-digit',
                    day: '2-digit',
                    hour: '2-digit',
                    minute: '2-digit'
                });

                const content = `
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <style>
                            table {
                                font-family: arial, sans-serif;
                                border-collapse: collapse;
                                width: 100%;
                            }
                            
                            td, th {
                                border: 1px solid #dddddd;
                                text-align: left;
                                padding: 8px;
                            }
                            
                            tr:nth-child(even) {
                                background-color: #dddddd;
                            }
                        </style>
                    </head>
                    <body>
                        <h2>Teklif Detay</h2>
                        <div>Teklif No: ${row.teklifNo}</div>
                        <div>Teklif Oluşturma Tarihi: ${formattedDate}</div>
                        <div>Teklif Geçerlilik Süresi: ${row.teklifSuresi} gün</div>
                        <br>
                        <table>
                            <tr>
                                <th>Ürün Adı</th>
                                <th>Adet</th>
                                <th>Birim Fiyat</th>
                                <th>Para Birimi</th>
                            </tr>
                            ${row.teklifItems
                                .map(
                                    (item) =>
                                        `<tr>
                                            <td>${item.urunAdi}</td>
                                            <td>${item.adet}</td>
                                            <td>${item.birimFiyat}</td>
                                            <td>${item.paraBirimi}</td>
                                        </tr>`
                                )
                                .join('')}
                                <tr>
                                    <th rowspan=3>KDV Dahil Toplam Fiyat</th>
                                    <th colspan=3>${row.toplamFiyatTRY}₺</th>
                                </tr>
                                <tr><th colspan=3>${row.toplamFiyatUSD}$</th></tr>
                                <tr><th colspan=3>${row.toplamFiyatEUR}€</th></tr>
                        </table>
                    </body>
                    </html>
                    `;
                printWindow.document.write(content);
                printWindow.document.close();
                printWindow.print();
            }
        });
    };

    return (
        <>
            <MaterialReactTable
                enableRowActions
                displayColumnDefOptions={{
                    'mrt-row-actions': {
                        header: 'İşlemler'
                    }
                }}
                renderRowActions={({ row }) => (
                    <Box sx={{ display: 'flex', flexWrap: 'nowrap', gap: '8px' }}>
                        <Tooltip title="Yazdır">
                            <IconButton color="info" onClick={() => handlePrint(row.original)}>
                                <PrintIcon />
                            </IconButton>
                        </Tooltip>
                        <Tooltip title="Sil">
                            <IconButton
                                color="error"
                                onClick={() => {
                                    deleteById(row.original.id, row.original.urunAdi, row.original.teklifDegeri);
                                }}
                            >
                                <DeleteIcon />
                            </IconButton>
                        </Tooltip>
                    </Box>
                )}
                positionActionsColumn="last"
                columns={columns}
                data={data !== undefined ? data.list : []} //data is undefined on first render
                renderDetailPanel={({ row }) => (
                    <Box
                        sx={{
                            display: 'grid',
                            margin: 'auto',
                            gridTemplateColumns: '1fr 1fr',
                            width: '100%'
                        }}
                    >
                        <MaterialReactTable
                            columns={[
                                {
                                    accessorKey: 'urunAdi',
                                    header: 'Ürün Adı'
                                },
                                {
                                    accessorKey: 'adet',
                                    header: 'Adet'
                                },
                                {
                                    accessorKey: 'birimFiyat',
                                    header: 'Birim Fiyat'
                                },
                                {
                                    accessorKey: 'paraBirimi',
                                    header: 'Para Birimi'
                                }
                            ]}
                            data={row.original.teklifItems.map((teklifItem) => ({
                                urunAdi: teklifItem.urunAdi,
                                adet: teklifItem.adet,
                                birimFiyat: teklifItem.birimFiyat,
                                paraBirimi: teklifItem.paraBirimi
                            }))}
                            muiTableProps={{
                                sx: {
                                    width: '500px',
                                    border: '1px solid rgba(81, 81, 81, 1)'
                                }
                            }}
                            muiTableHeadCellProps={{
                                sx: {
                                    border: '1px solid rgba(81, 81, 81, 1)'
                                }
                            }}
                            muiTableBodyCellProps={{
                                sx: {
                                    border: '1px solid rgba(81, 81, 81, 1)'
                                }
                            }}
                        />
                    </Box>
                )}
                muiToolbarAlertBannerProps={
                    isError
                        ? {
                              color: 'error',
                              children: 'Error loading data'
                          }
                        : undefined
                }
                onColumnFiltersChange={setColumnFilters}
                onGlobalFilterChange={setGlobalFilter}
                onPaginationChange={setPagination}
                onSortingChange={setSorting}
                renderTopToolbarCustomActions={() => (
                    <Tooltip arrow title="Refresh Data">
                        <IconButton onClick={() => refetch()}>
                            <RefreshIcon />
                        </IconButton>
                    </Tooltip>
                )}
                rowCount={data?.dataCount ?? 0}
                state={{
                    columnFilters,
                    globalFilter,
                    isLoading,
                    pagination,
                    showAlertBanner: isError,
                    showProgressBars: isFetching,
                    sorting
                }}
            />
        </>
    );
};

const queryClient = new QueryClient();

const ExampleWithReactQueryProvider = () => (
    <QueryClientProvider client={queryClient}>
        <Example />
    </QueryClientProvider>
);

export default ExampleWithReactQueryProvider;
