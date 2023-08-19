import { useMemo, useState } from 'react';
import MaterialReactTable from 'material-react-table';
import {
    Box,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    IconButton,
    Tooltip,
    Button,
    DialogContentText,
    InputAdornment,
    Input,
    FormControl
} from '@mui/material';
import RefreshIcon from '@mui/icons-material/Refresh';
import { QueryClient, QueryClientProvider, useQuery } from '@tanstack/react-query';
import axios from 'axios';
import { AddCircle as AddCircleIcon, Edit as EditIcon, Delete as DeleteIcon, LocalOffer as LocalOfferIcon } from '@mui/icons-material';
import { toast } from 'react-toastify';
import { useNavigate } from 'react-router-dom';

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

    const [urunAdi, setUrunAdi] = useState('');
    const [urunId, setUrunId] = useState('');
    const [teklifDegeri, setTeklifDegeri] = useState('');
    const [musteriId, setMusteriId] = useState('');
    const [openDialog, setOpenDialog] = useState(false);
    const [offer, setOffer] = useState('');
    const [validationErrors, setValidationErrors] = useState({});

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
                url: 'http://localhost:5273/api/Urun/GetGrid',
                data: data
            };

            await axios
                .request(config)
                .then((response) => {
                    console.log(response.data.data.list);
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
                accessorKey: 'adi',
                header: 'İsim'
            },
            {
                accessorKey: 'aciklama',
                header: 'Açıklama'
            },
            {
                accessorKey: 'ebat',
                header: 'Ebat'
            },
            {
                accessorKey: 'fiyat',
                header: 'Fiyat'
            },
            {
                accessorKey: 'paraBirimi',
                header: 'Para Birimi'
            },
            {
                accessorKey: 'tedarikci',
                header: 'Tedarikçi Firma'
            },
            {
                accessorKey: 'kdv',
                header: 'KDV Oranı'
            },
            {
                accessorKey: 'kategori',
                header: 'Kategori'
            },
            {
                accessorKey: 'urunSahibi',
                header: 'Ürün Sahibi'
            }
        ],
        []
    );

    const deleteById = (id) => {
        toast.promise(deletePromise(id), {
            pending: 'Ürün siliniyor.',
            success: 'Ürün başarıyla silindi 👌',
            error: 'Ürün silinirken hata oluştu 🤯'
        });
    };

    const deletePromise = (id) => {
        return new Promise(async (resolve, reject) => {
            let config = {
                method: 'get',
                maxBodyLength: Infinity,
                url: 'http://localhost:5273/api/Urun/Delete',
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

    const teklifKontrol = (id, ad, sahibi) => {
        return new Promise(async (resolve, reject) => {
            let config = {
                method: 'post',
                maxBodyLength: Infinity,
                url: 'http://localhost:5273/api/Teklif/TeklifKontrol',
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
                        if (response.data.data == 1) {
                            toast.warn('Bu ürüne zaten teklif yaptınız: "' + ad + '"');
                        } else {
                            navigate(`/digerIslemler/teklif-ver/${id}`, {
                                state: {
                                    urunAdi: ad,
                                    urunSahibi: sahibi
                                }
                            });
                        }
                        resolve(response.data);
                    } else {
                        reject(new Error('İşlem başarısız'));
                    }
                })
                .catch((error) => {
                    console.log(error);
                    reject(error);
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
                        <Tooltip title="Teklif ver">
                            <IconButton
                                color="primary"
                                onClick={() => {
                                    teklifKontrol(row.original.id, row.original.adi, row.original.urunSahibi);
                                }}
                            >
                                <AddCircleIcon />
                            </IconButton>
                        </Tooltip>
                        <Tooltip title="Düzenle">
                            <IconButton
                                color="secondary"
                                onClick={() => {
                                    navigate(`/digerIslemler/urun-duzenle/${row.original.id}`);
                                }}
                            >
                                <EditIcon />
                            </IconButton>
                        </Tooltip>
                        <Tooltip title="Sil">
                            <IconButton
                                color="error"
                                onClick={() => {
                                    deleteById(row.original.id);
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
