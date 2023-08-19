import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate, useParams } from 'react-router';
import { useQuery, QueryClientProvider, QueryClient } from '@tanstack/react-query';
import { useMemo } from 'react';
import MaterialReactTable from 'material-react-table';
import { Box, IconButton, Tooltip } from '@mui/material';
import { Refresh as RefreshIcon, Edit as EditIcon, Delete as DeleteIcon, Email as EmailIcon } from '@mui/icons-material';
import { toast } from 'react-toastify';

const Example = () => {
    const sleep = (delay) => new Promise((resolve) => setTimeout(resolve, delay));
    const navigate = useNavigate();

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
                accessorKey: 'urunAdi',
                header: 'Ürün Adı'
            },
            {
                accessorKey: 'urunSahibi',
                header: 'Ürün Sahibi'
            },
            {
                accessorKey: 'teklifDegeri',
                header: 'Teklif Miktarı'
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
                        <Tooltip title="Düzenle">
                            <IconButton
                                color="secondary"
                                onClick={() => {
                                    navigate(`/digerIslemler/teklif-duzenle/${row.original.id}`, {
                                        state: {
                                            urunAdi: row.original.urunAdi,
                                            urunSahibi: row.original.urunSahibi
                                        }
                                    });
                                }}
                            >
                                <EditIcon />
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
