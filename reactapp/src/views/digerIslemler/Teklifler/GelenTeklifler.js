import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useParams } from 'react-router';
import { useQuery, QueryClientProvider, QueryClient } from '@tanstack/react-query';
import { useMemo } from 'react';
import MaterialReactTable from 'material-react-table';
import { Box, IconButton, Tooltip } from '@mui/material';
import { Refresh as RefreshIcon, ThumbDown as ThumbDownIcon, ThumbUp as ThumbUpIcon } from '@mui/icons-material';

const Example = () => {
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
                url: 'http://localhost:5273/api/Teklif/GetGelenTeklifler',
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
                accessorKey: 'teklifVeren',
                header: 'Teklif Veren'
            },
            {
                accessorKey: 'teklifDegeri',
                header: 'Teklif Miktarı'
            }
        ],
        []
    );

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
                        <Tooltip title="Kabul Et">
                            <IconButton color="success" onClick={() => alert('kabul')}>
                                <ThumbUpIcon />
                            </IconButton>
                        </Tooltip>
                        <Tooltip title="Reddet">
                            <IconButton color="error" onClick={() => alert('ret')}>
                                <ThumbDownIcon />
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
